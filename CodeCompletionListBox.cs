/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Portions Copyright ©2007-2010 BoltBait. All Rights Reserved.
// Portions Copyright ©2017-2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©2004 yetanotherchris.  All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE CODELAB DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////
// Intellisense code from http://www.codeproject.com/KB/cs/diy-intellisense.aspx
// which required tons of fixes in order to get it to work properly.
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class IntelliBox : ListBox
    {
        private bool mouseOver;
        private bool filterMatches;
        private readonly Size iconSize;
        private readonly IReadOnlyList<Image> itemIcons;
        private readonly IntelliTip itemToolTip = new IntelliTip();
        private readonly List<IntelliBoxItem> unFilteredItems = new List<IntelliBoxItem>();
        private IntelliBoxItem LastUsedMember = IntelliBoxItem.Empty;
        private IntelliBoxItem LastUsedNonMember = IntelliBoxItem.Empty;
        private IntelliBoxContents intelliBoxContents;
        private string stringFilter = string.Empty;
        private IntelliType intelliTypeFilter = IntelliType.None;

        internal string AutoCompleteCode => SelectedItem.ToString();
        internal bool MouseOver => mouseOver;
        internal bool Matches => filterMatches;
        internal int IconWidth => iconSize.Width + 2;
        internal bool ExtraSpace => (intelliBoxContents != IntelliBoxContents.NonMembers && intelliBoxContents != IntelliBoxContents.Constructors);

        internal IntelliBox()
        {
            // Set owner draw mode
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = UIUtil.Scale(16);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Cursor = Cursors.Default;

            iconSize = UIUtil.ScaleSize(16, 16);
            itemIcons = new Image[]
            {
                UIUtil.GetImage("Method"),
                UIUtil.GetImage("Property"),
                UIUtil.GetImage("Event"),
                UIUtil.GetImage("Field"),
                UIUtil.GetImage("Keyword"),
                UIUtil.GetImage("Type"),
                UIUtil.GetImage("Var"),
                UIUtil.GetImage("Class"),
                UIUtil.GetImage("Struct"),
                UIUtil.GetImage("Enum"),
                UIUtil.GetImage("Const"),
                UIUtil.GetImage("EnumItem"),
                UIUtil.GetImage("Snippet"),
                UIUtil.GetImage("Method"), // Use the Method icon for Constructor
                UIUtil.GetImage("Var"), // Use the Variable icon for Parameter
                UIUtil.GetImage("Interface"),
                UIUtil.GetImage("Delegate"),
                UIUtil.EmptyImage
            };
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            itemToolTip.Hide(this);

            base.OnLocationChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseOver = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseOver = false;
            base.OnMouseLeave(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            ((CodeTextBox)Parent).ConfirmIntelliBox();
            base.OnDoubleClick(e);
        }

        protected override void OnClick(EventArgs e)
        {
            Parent.Focus();
            base.OnClick(e);
        }

        /// <summary>
        /// Fills the iBox with members of the given Type.
        /// </summary>
        internal void PopulateMembers(Type type, bool isStatic)
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.Members;

            BindingFlags bindingFlags = isStatic ?
                BindingFlags.Static | BindingFlags.Public :
                BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance | BindingFlags.Public;

            MemberInfo[] members = type.GetMembers(bindingFlags);
            foreach (MemberInfo memberInfo in members)
            {
                if ((!memberInfo.ReflectedType.IsVisible && memberInfo.ReflectedType?.DeclaringType?.FullName != Intelli.UserScriptFullName) ||
                    memberInfo.ReflectedType.IsSpecialName || memberInfo.IsObsolete())
                {
                    continue;
                }

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Method:
                        MethodInfo methodInfo = (MethodInfo)memberInfo;
                        if (methodInfo.IsSpecialName || (type.IsArray && methodInfo.DeclaringType == type))
                        {
                            continue;
                        }

                        AddMethod(methodInfo, false);
                        break;
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)memberInfo;
                        if (property.IsIndexer())
                        {
                            continue;
                        }

                        AddProperty(property);
                        break;
                    case MemberTypes.Event:
                        EventInfo eventInfo = (EventInfo)memberInfo;

                        AddEvent(eventInfo);
                        break;
                    case MemberTypes.Field:
                        FieldInfo field = (FieldInfo)memberInfo;
                        if (field.IsSpecialName)
                        {
                            continue;
                        }

                        AddField(field);
                        break;
                    case MemberTypes.NestedType:
                        if (!isStatic)
                        {
                            continue;
                        }

                        Type nestedType = (Type)memberInfo;

                        AddType(nestedType, nestedType.Name, true);
                        break;
                }
            }

            if (!isStatic)
            {
                foreach (MethodInfo methodInfo in type.GetExtensionMethods())
                {
                    AddMethod(methodInfo, true);
                }
            }

            unFilteredItems.Sort();
            this.Items.AddRange(unFilteredItems.ToArray());

            if (this.Items.Contains(this.LastUsedMember))
            {
                this.SelectedItem = this.LastUsedMember;
            }
            else if (this.Items.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Fills the iBox with all non-member objects, and then filters them with the given char.
        /// </summary>
        internal void PopulateNonMembers(char startChar, bool inClassRoot)
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.NonMembers;

            foreach (string key in Intelli.Keywords)
            {
                string toolTip = $"{key}\nKeyword";
                unFilteredItems.Add(new IntelliBoxItem(key, key, toolTip, IntelliType.Keyword));
            }

            foreach (string snip in Intelli.Snippets.Keys)
            {
                string toolTip = $"{snip}\nSnippet - Tab Twice";
                unFilteredItems.Add(new IntelliBoxItem(snip, snip, toolTip, IntelliType.Snippet));
            }

            foreach (string typeName in Intelli.AutoCompleteTypes.Keys)
            {
                Type type = Intelli.AutoCompleteTypes[typeName];
                AddType(type, typeName, false);
            }

            foreach (string typeName in Intelli.UserDefinedTypes.Keys)
            {
                Type type = Intelli.UserDefinedTypes[typeName];
                AddType(type, typeName, null);
            }

            if (!inClassRoot)
            {
                foreach (string var in Intelli.Variables.Keys)
                {
                    string type = Intelli.Variables[var].GetDisplayName();
                    string toolTip = $"{type} - {var}\nLocal Variable";
                    unFilteredItems.Add(new IntelliBoxItem(var, var, toolTip, IntelliType.Variable));
                }

                foreach (string para in Intelli.Parameters.Keys)
                {
                    string type = Intelli.Parameters[para].GetDisplayName();
                    string toolTip = $"{type} - {para}\nParameter";
                    unFilteredItems.Add(new IntelliBoxItem(para, para, toolTip, IntelliType.Parameter));
                }

                MemberInfo[] members = Intelli.UserScript.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MemberInfo memberInfo in members)
                {
                    if (memberInfo.IsObsolete())
                    {
                        continue;
                    }

                    switch (memberInfo.MemberType)
                    {
                        case MemberTypes.Method:
                            MethodInfo methodInfo = (MethodInfo)memberInfo;
                            if (methodInfo.IsSpecialName || methodInfo.DeclaringType != Intelli.UserScript ||
                                methodInfo.Name.Equals("Render", StringComparison.Ordinal) || methodInfo.Name.Equals("PreRender", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            AddMethod(methodInfo, false);
                            break;
                        case MemberTypes.Property:
                            PropertyInfo property = (PropertyInfo)memberInfo;
                            if (property.Name.Equals("SetRenderInfoCalled", StringComparison.Ordinal) || property.Name.Equals("__DebugMsgs", StringComparison.Ordinal) ||
                                property.IsIndexer())
                            {
                                continue;
                            }

                            AddProperty(property);
                            break;
                        case MemberTypes.Event:
                            EventInfo eventInfo = (EventInfo)memberInfo;

                            AddEvent(eventInfo);
                            break;
                        case MemberTypes.Field:
                            if (memberInfo.Name.Equals("RandomNumber", StringComparison.Ordinal) || memberInfo.Name.Equals("instanceSeed", StringComparison.Ordinal) ||
                                memberInfo.Name.EndsWith("_BackingField", StringComparison.Ordinal) ||
                                memberInfo.Name.Equals("__listener", StringComparison.Ordinal) || memberInfo.Name.Equals("__debugWriter", StringComparison.Ordinal))
                            {
                                continue;
                            }

                            FieldInfo field = (FieldInfo)memberInfo;

                            AddField(field);
                            break;
                    }
                }
            }

            unFilteredItems.Sort();
            Filter(startChar.ToString().Trim());
        }

        /// <summary>
        /// Fills the iBox with all the Constructors of the given Type.
        /// </summary>
        internal void PopulateConstructors(Type type)
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.Constructors;

            if (type.IsValueType)
            {
                unFilteredItems.Add(new IntelliBoxItem($"{type.Name}()", string.Empty, $"{type.Name}()", IntelliType.Constructor));
            }

            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                string toolTip = $"{type.Name}({constructor.Params()})";
                unFilteredItems.Add(new IntelliBoxItem(toolTip, string.Empty, toolTip + "\nConstructor", IntelliType.Constructor));
            }

            this.Items.AddRange(unFilteredItems.ToArray());
            if (unFilteredItems.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        internal void PopulateSuggestions(Type type)
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.Suggestions;

            bool plural = false;
            Type iEnumType = type.GetInterface("IEnumerable`1") ?? ((type.IsInterface && type.Name.Equals("IEnumerable`1", StringComparison.Ordinal)) ? type : null);
            if (iEnumType != null && iEnumType.IsConstructedGenericType && iEnumType.GenericTypeArguments.Length == 1)
            {
                plural = true;
                type = iEnumType.GenericTypeArguments[0];
            }

            string typeName = Regex.Replace(type.Name, @"`\d", string.Empty);

            unFilteredItems.AddRange(SplitCamelCase(typeName)
                .Select(str =>
                    {
                        string suggestion = plural ? str.MakePlural() : str;
                        return new IntelliBoxItem(suggestion, suggestion, "(Suggested name)", IntelliType.Variable);
                    })
                );

            this.Items.AddRange(unFilteredItems.ToArray());
            this.SelectedIndex = 0;
        }

        internal void PopulateXamlTags()
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.XamlTags;

            foreach (string typeName in Intelli.XamlAutoCompleteTypes.Keys)
            {
                Type type = Intelli.XamlAutoCompleteTypes[typeName];
                AddType(type, typeName, false);
            }

            unFilteredItems.Sort();
            this.Items.AddRange(unFilteredItems.ToArray());

            if (this.Items.Contains(this.LastUsedNonMember))
            {
                this.SelectedItem = this.LastUsedNonMember;
            }
            else if (this.Items.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        internal void PopulateXamlAttributes(Type tagType)
        {
            Clear();
            this.intelliBoxContents = IntelliBoxContents.XamlAttributes;

            PropertyInfo[] properties = tagType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            bool isSelfClosing = true;

            foreach (PropertyInfo property in properties)
            {
                if (!property.ReflectedType.IsVisible || !property.CanWrite || property.ReflectedType.IsSpecialName || property.IsObsolete() || property.IsIndexer())
                {
                    continue;
                }

                if (typeof(System.Collections.IList).IsAssignableFrom(property.PropertyType))
                {
                    isSelfClosing = false;
                    continue;
                }

                AddProperty(property);
            }

            unFilteredItems.Sort();
            if (isSelfClosing)
            {
                unFilteredItems.Add(new IntelliBoxItem("/> Close Tag", "/>", "/> Close Tag", IntelliType.None));
            }

            this.Items.AddRange(unFilteredItems.ToArray());

            if (this.Items.Contains(this.LastUsedMember))
            {
                this.SelectedItem = this.LastUsedMember;
            }
            else if (this.Items.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        private void Clear()
        {
            this.Items.Clear();
            this.unFilteredItems.Clear();
            this.stringFilter = string.Empty;
            this.intelliTypeFilter = IntelliType.None;
        }

        private void AddMethod(MethodInfo methodInfo, bool isExtension)
        {
            string returnType = methodInfo.ReturnType.GetDisplayName();
            string byRef = methodInfo.ReturnType.IsByRef ? "ref " : string.Empty;
            string methodParameters = $"({methodInfo.Params()})";
            string genericArgs = string.Empty;
            string genericContraints = string.Empty;
            string ext = isExtension ? "Extension " : string.Empty;

            if (methodInfo.IsGenericMethod)
            {
                Type[] args = methodInfo.GetGenericArguments();
                genericArgs = $"<{args.Select(t => t.GetDisplayName()).Join(", ")}>";

                if (methodInfo.IsGenericMethodDefinition)
                {
                    string constraints = args.GetConstraints().Join("\r\n    ");
                    if (constraints.Length > 0)
                    {
                        genericContraints = "\r\n    " + constraints;
                    }
                }
            }

            string toolTip = $"{byRef}{returnType} - {methodInfo.Name}{genericArgs}{methodParameters}{genericContraints}\n{ext}{methodInfo.MemberType}";
            unFilteredItems.Add(new IntelliBoxItem(methodInfo.Name + genericArgs + methodParameters, methodInfo.Name, toolTip, IntelliType.Method));
        }

        private void AddProperty(PropertyInfo property)
        {
            string returnType = property.PropertyType.GetDisplayName();
            string getSet = property.GetterSetter();
            string toolTip = $"{returnType} - {property.Name}{getSet}\n{property.MemberType}";
            unFilteredItems.Add(new IntelliBoxItem(property.Name, property.Name, toolTip, IntelliType.Property));
        }

        private void AddEvent(EventInfo eventInfo)
        {
            string returnType = eventInfo.EventHandlerType.GetDisplayName();
            string toolTip = $"{returnType} - {eventInfo.Name}\n{eventInfo.MemberType}";
            unFilteredItems.Add(new IntelliBoxItem(eventInfo.Name, eventInfo.Name, toolTip, IntelliType.Event));
        }

        private void AddField(FieldInfo field)
        {
            string returnType = field.FieldType.GetDisplayName();
            string fieldTypeName;
            IntelliType fieldType;
            string fieldValue;

            if (!field.IsStatic)
            {
                fieldTypeName = "Field";
                fieldType = IntelliType.Field;
                fieldValue = string.Empty;
            }
            else if (field.FieldType.IsEnum)
            {
                fieldTypeName = "Enum Value";
                fieldType = IntelliType.EnumItem;
                fieldValue = $" ({field.GetEnumValue()})";
            }
            else if (field.IsLiteral && !field.IsInitOnly)
            {
                fieldTypeName = "Constant";
                fieldType = IntelliType.Constant;
                fieldValue = $" ({field.GetConstValue()})";
            }
            else
            {
                fieldTypeName = "Field";
                fieldType = IntelliType.Field;
                fieldValue = $" ( {field.GetValue(null)} )";
            }

            string toolTip = $"{returnType} - {field.Name}{fieldValue}\n{fieldTypeName}";
            unFilteredItems.Add(new IntelliBoxItem(field.Name, field.Name, toolTip, fieldType));
        }

        private void AddType(Type type, string typeName, bool? isNested)
        {
            string realName = type.Name;
            string name = typeName;
            string code = typeName;

            if (type.IsGenericType)
            {
                realName = type.GetGenericName();
                if (typeName.Equals(type.Name, StringComparison.Ordinal))
                {
                    name = type.GetGenericName();
                    code = Regex.Replace(type.Name, @"`\d", string.Empty);
                }

                if (type.IsGenericTypeDefinition)
                {
                    string constraints = type.GetGenericArguments().GetConstraints().Join("\r\n    ");
                    if (constraints.Length > 0)
                    {
                        realName += "\r\n    " + constraints;
                    }
                }
            }

            string baseType = "Type";
            IntelliType icon = IntelliType.Type;

            if (type.IsEnum)
            {
                baseType = "enum";
                icon = IntelliType.Enum;
            }
            else if (type.IsValueType)
            {
                baseType = "struct";
                icon = IntelliType.Struct;
            }
            else if (type.IsDelegate())
            {
                baseType = "delegate";
                icon = IntelliType.Delegate;
            }
            else if (type.IsClass)
            {
                baseType = "class";
                icon = IntelliType.Class;
            }
            else if (type.IsInterface)
            {
                baseType = "interface";
                icon = IntelliType.Interface;
            }

            string toolTip;
            if (isNested == null)
            {
                toolTip = $"{baseType} - {type.DeclaringType.Name}.{realName}\nType"; // User Defined
            }
            else if (isNested == true)
            {
                toolTip = $"{baseType} - {realName}\nNested Type";
            }
            else
            {
                toolTip = $"{baseType} - {type.Namespace}.{realName}\nType";
            }

            unFilteredItems.Add(new IntelliBoxItem(name, code, toolTip, icon));
        }

        private static IEnumerable<string> SplitCamelCase(string str)
        {
            List<string> results = new List<string>();
            results.Add(str.FirstCharToLower());

            string[] splitCamelCase = Regex.Split(str, "(?<!^)(?=[A-Z])");
            for (int i = splitCamelCase.Length - 1; i > 0; i--)
            {
                StringBuilder subtractEnd = new StringBuilder(i);
                for (int j = 0; j < i; j++)
                {
                    subtractEnd.Append(splitCamelCase[j]);
                }
                results.Add(subtractEnd.ToString().FirstCharToLower());

                StringBuilder subtractStart = new StringBuilder(i);
                for (int j = splitCamelCase.Length - i; j < splitCamelCase.Length; j++)
                {
                    subtractStart.Append(splitCamelCase[j]);
                }
                results.Add(subtractStart.ToString().FirstCharToLower());
            }

            return results;
        }

        internal void Filter(string contains)
        {
            stringFilter = contains;

            Filter();
        }

        internal void Filter(IntelliType intelliTypes)
        {
            intelliTypeFilter = (intelliTypes == intelliTypeFilter) ? IntelliType.None : intelliTypes;

            Filter();
        }

        private void Filter()
        {
            List<IntelliBoxItem> matches = new List<IntelliBoxItem>();
            foreach (IntelliBoxItem item in unFilteredItems)
            {
                if (intelliTypeFilter != IntelliType.None && item.ImageIndex != (int)intelliTypeFilter)
                {
                    continue;
                }

                string itemName = item.ToString();
                if (!itemName.Contains(stringFilter, StringComparison.OrdinalIgnoreCase) && !itemName.GetInitials().Contains(stringFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matches.Add(item);
            }

            if (intelliTypeFilter == IntelliType.None && matches.Count == 0)
            {
                filterMatches = false;
                return;
            }

            this.Items.Clear();

            if (matches.Count == 0)
            {
                filterMatches = false;
                return;
            }

            filterMatches = true;
            this.Items.AddRange(matches.ToArray());


            if (stringFilter.Length == 0)
            {
                if (this.Items.Contains(LastUsedMember))
                {
                    this.SelectedItem = LastUsedMember;
                }
                else if (this.Items.Contains(LastUsedNonMember))
                {
                    this.SelectedItem = LastUsedNonMember;
                }
                else
                {
                    this.SelectedIndex = 0;
                }

                return;
            }

            int indexToSelect = this.FindStringExact(stringFilter, true);
            if (indexToSelect == -1)
            {
                if (this.Items.Contains(LastUsedMember) && LastUsedMember.ToString().StartsWith(stringFilter, StringComparison.Ordinal))
                {
                    indexToSelect = this.FindStringExact(LastUsedMember.ToString(), false);
                }
                else if (this.Items.Contains(LastUsedNonMember) && LastUsedNonMember.ToString().StartsWith(stringFilter, StringComparison.Ordinal))
                {
                    indexToSelect = this.FindStringExact(LastUsedNonMember.ToString(), false);
                }

                if (indexToSelect == -1)
                {
                    indexToSelect = this.FindString(stringFilter, true);
                    if (indexToSelect == -1)
                    {
                        indexToSelect = 0;
                    }
                }
            }

            this.SelectedIndex = indexToSelect;
            this.TopIndex = indexToSelect;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            e.DrawBackground();
            e.DrawFocusRectangle();

            if (Items[e.Index] is IntelliBoxItem item)
            {
                using (SolidBrush iconBg = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(iconBg, e.Bounds.Left, e.Bounds.Top, iconSize.Width + 1, iconSize.Height);
                }
                e.Graphics.DrawImage(itemIcons[item.ImageIndex], e.Bounds.Left, e.Bounds.Top, iconSize.Width, iconSize.Height);
                TextRenderer.DrawText(e.Graphics, item.Text, e.Font, new Point(e.Bounds.Left + iconSize.Width, e.Bounds.Top), e.ForeColor);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, new Point(e.Bounds.Left, e.Bounds.Top), e.ForeColor);
            }

            base.OnDrawItem(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.Visible && this.SelectedItem is IntelliBoxItem item)
            {
                itemToolTip.Show(item.ToolTip, this, this.Width, 0);
            }
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible)
            {
                itemToolTip.Hide(this);
            }
            else if (this.SelectedItem is IntelliBoxItem item)
            {
                itemToolTip.Show(item.ToolTip, this, this.Width, 0);
            }

            base.OnVisibleChanged(e);
        }

        internal void UpdateTheme(Color toolTipFore, Color toolTipBack)
        {
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            itemToolTip.UpdateTheme(toolTipFore, toolTipBack);
        }

        internal void SaveUsedItem()
        {
            if (this.SelectedItem is IntelliBoxItem item)
            {
                if (this.intelliBoxContents == IntelliBoxContents.Members || this.intelliBoxContents == IntelliBoxContents.XamlAttributes)
                {
                    this.LastUsedMember = item;
                }
                else if ((this.intelliBoxContents == IntelliBoxContents.NonMembers || this.intelliBoxContents == IntelliBoxContents.XamlTags) &&
                    item.ImageIndex != (int)IntelliType.Keyword && item.ImageIndex != (int)IntelliType.Snippet)
                {
                    this.LastUsedNonMember = item;
                }
            }
        }

        internal void FindAndSelect(string itemName)
        {
            int itemIndex = this.FindStringExact(itemName, true);
            if (itemIndex == -1)
            {
                itemIndex = this.FindString(itemName, true);
                if (itemIndex == -1)
                {
                    return;
                }
            }

            this.SelectedIndex = itemIndex;
            this.TopIndex = itemIndex;
        }

        private class IntelliBoxItem : IComparable<IntelliBoxItem>, IEquatable<IntelliBoxItem>
        {
            public int CompareTo(IntelliBoxItem other)
            {
                return string.Compare(this.Text, other.Text, StringComparison.OrdinalIgnoreCase);
            }
            public bool Equals(IntelliBoxItem other)
            {
                return (this.Text == other.Text && this.Code == other.Code && this.ToolTip == other.ToolTip && this.ImageIndex == other.ImageIndex);
            }
            public override bool Equals(object obj)
            {
                if (obj is IntelliBoxItem other)
                {
                    return (this.Text == other.Text && this.Code == other.Code && this.ToolTip == other.ToolTip && this.ImageIndex == other.ImageIndex);
                }

                return false;
            }
            public override int GetHashCode()
            {
                return this.Text.GetHashCode() ^ this.Code.GetHashCode() ^ this.ToolTip.GetHashCode() ^ this.ImageIndex.GetHashCode();
            }

            internal static readonly IntelliBoxItem Empty = new IntelliBoxItem(string.Empty, string.Empty, string.Empty, IntelliType.None);

            internal string Text { get; }
            internal string Code { get; }
            internal string ToolTip { get; }
            internal int ImageIndex { get; }

            internal IntelliBoxItem(string text, string code, string toolTip, IntelliType intelliType)
            {
                this.Text = text;
                this.Code = code;
                this.ToolTip = toolTip;
                this.ImageIndex = (int)intelliType;
            }

            public override string ToString()
            {
                return Code;
            }
        }

        private enum IntelliBoxContents
        {
            Members,
            NonMembers,
            Constructors,
            Suggestions,
            XamlTags,
            XamlAttributes
        }
    }
}
