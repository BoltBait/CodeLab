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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class IntelliBox : ListBox
    {
        private readonly ImageList imageList = new ImageList();
        private bool mouseOver;
        private bool filterMatches;
        private readonly IntelliTip itemToolTip = new IntelliTip();
        private readonly List<IntelliBoxItem> unFilteredItems = new List<IntelliBoxItem>();
        private IntelliBoxItem LastUsedMember = IntelliBoxItem.Empty;
        private IntelliBoxItem LastUsedNonMember = IntelliBoxItem.Empty;
        private bool membersInBox;
        private string stringFilter = string.Empty;
        private IntelliType intelliTypeFilter = IntelliType.None;

        internal bool MouseOver => mouseOver;
        internal bool Matches => filterMatches;
        internal int IconWidth => imageList.ImageSize.Width + 2;

        internal IntelliBox()
        {
            SizeF dpi;
            using (Graphics g = this.CreateGraphics())
                dpi = new SizeF(g.DpiX / 96f, g.DpiY / 96f);

            // Set owner draw mode
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = (int)Math.Round(16 * dpi.Height);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Cursor = Cursors.Default;

            imageList.ImageSize = new Size((int)Math.Round(16 * dpi.Width), (int)Math.Round(16 * dpi.Height));
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.AddRange(new Image[]
            {
                ResUtil.GetImage("Method"),
                ResUtil.GetImage("Property"),
                ResUtil.GetImage("Event"),
                ResUtil.GetImage("Field"),
                ResUtil.GetImage("Keyword"),
                ResUtil.GetImage("Type"),
                ResUtil.GetImage("Var"),
                ResUtil.GetImage("Class"),
                ResUtil.GetImage("Struct"),
                ResUtil.GetImage("Enum"),
                ResUtil.GetImage("Const"),
                ResUtil.GetImage("EnumItem"),
                ResUtil.GetImage("Snippet"),
                ResUtil.GetImage("Method"), // Use the Method icon for Constructor
                ResUtil.GetImage("Var"), // Use the Variable icon for Parameter
                ResUtil.GetImage("Interface")
            });
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
        internal void Populate(Type type, bool isStatic)
        {
            this.Items.Clear();
            unFilteredItems.Clear();
            stringFilter = string.Empty;
            intelliTypeFilter = IntelliType.None;
            membersInBox = true;

            MemberInfo[] members = isStatic ?
                type.GetMembers(BindingFlags.Static | BindingFlags.Public) :
                type.GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance | BindingFlags.Public);

            foreach (MemberInfo memberInfo in members)
            {
                if ((!memberInfo.ReflectedType.IsVisible && memberInfo.DeclaringType?.DeclaringType?.FullName != Intelli.UserScriptFullName) ||
                    memberInfo.ReflectedType.IsSpecialName || memberInfo.IsDefined(typeof(ObsoleteAttribute), false))
                {
                    continue;
                }

                string returnType = string.Empty;
                string toolTip = string.Empty;

                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Method:
                        MethodInfo methodInfo = (MethodInfo)memberInfo;
                        if (methodInfo.IsSpecialName)
                        {
                            continue;
                        }

                        AddMethod(methodInfo, false);
                        break;
                    case MemberTypes.Property:
                        PropertyInfo property = (PropertyInfo)memberInfo;
                        if (property.GetIndexParameters().Length > 0)
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

                        AddField(field);
                        break;
                    case MemberTypes.NestedType:
                        if (!isStatic)
                        {
                            continue;
                        }

                        Type nestedType = (Type)memberInfo;

                        AddType(nestedType, true);
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
            else if (this.Items.Contains(this.LastUsedNonMember))
            {
                this.SelectedItem = this.LastUsedNonMember;
            }
            else if (this.Items.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Fills the iBox with all non-member objects, and then filters them with the given char.
        /// </summary>
        internal void Populate(char startChar)
        {
            this.Items.Clear();
            unFilteredItems.Clear();
            stringFilter = string.Empty;
            intelliTypeFilter = IntelliType.None;
            membersInBox = false;

            foreach (string type in Intelli.AutoCompleteTypes.Keys)
            {
                Type t = Intelli.AutoCompleteTypes[type];
                AddType(t, false);
            }

            foreach (string type in Intelli.UserDefinedTypes.Keys)
            {
                Type t = Intelli.UserDefinedTypes[type];
                AddType(t, null);
            }

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

            MemberInfo[] members = Intelli.UserScript.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MemberInfo memberInfo in members)
            {
                if (memberInfo.IsDefined(typeof(ObsoleteAttribute), false))
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
                            property.GetIndexParameters().Length > 0)
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

            unFilteredItems.Sort();
            Filter(startChar.ToString().Trim());
        }

        /// <summary>
        /// Fills the iBox with all the Constructors of the given Type.
        /// </summary>
        internal void Populate(Type type)
        {
            this.Items.Clear();
            unFilteredItems.Clear();
            stringFilter = string.Empty;
            intelliTypeFilter = IntelliType.None;
            membersInBox = false;

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

        private void AddMethod(MethodInfo methodInfo, bool isExtension)
        {
            string returnType = methodInfo.ReturnType.GetDisplayName();
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
                    genericContraints = args.GetConstraints();
                }
            }

            string toolTip = $"{returnType} - {methodInfo.Name}{genericArgs}{methodParameters}{genericContraints}\n{ext}{methodInfo.MemberType}";
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
                fieldValue = $" ({field.GetValue(null)})";
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

        private void AddType(Type type, bool? isNested)
        {
            string realName = type.Name;
            string name = type.Name;
            string code = type.Name;

            if (type.IsGenericType)
            {
                realName = type.GetGenericName();
                name = type.GetGenericName();
                code = Regex.Replace(type.Name, @"`\d", string.Empty) + "<>";

                if (type.IsGenericTypeDefinition)
                {
                    realName += type.GetGenericArguments().GetConstraints();
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
                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].ToString().StartsWith(stringFilter, StringComparison.Ordinal))
                    {
                        this.SelectedIndex = i;
                        break;
                    }
                }

                if (this.SelectedIndex == -1)
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        if (this.Items[i].ToString().StartsWith(stringFilter, StringComparison.OrdinalIgnoreCase))
                        {
                            this.SelectedIndex = i;
                            break;
                        }
                    }
                }

                if (this.SelectedIndex == -1)
                {
                    this.SelectedIndex = 0;
                }
            }
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
                    e.Graphics.FillRectangle(iconBg, e.Bounds.Left, e.Bounds.Top, imageList.ImageSize.Width + 1, imageList.ImageSize.Height);
                imageList.Draw(e.Graphics, e.Bounds.Left, e.Bounds.Top, item.ImageIndex);
                TextRenderer.DrawText(e.Graphics, item.Text, e.Font, new Point(e.Bounds.Left + imageList.ImageSize.Width, e.Bounds.Top), e.ForeColor);
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

        internal void HideToolTip()
        {
            itemToolTip.Hide(this);
        }

        internal void SaveUsedItem()
        {
            if (this.SelectedItem is IntelliBoxItem item)
            {
                if (membersInBox)
                {
                    this.LastUsedMember = item;
                }
                else if (item.ImageIndex != (int)IntelliType.Keyword && item.ImageIndex != (int)IntelliType.Snippet)
                {
                    this.LastUsedNonMember = item;
                }
            }
        }

        internal void FindAndSelect(string itemName)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is IntelliBoxItem intelliItem &&
                    intelliItem.Text.Equals(itemName, StringComparison.OrdinalIgnoreCase))
                {
                    this.SelectedIndex = i;
                    this.TopIndex = i;
                    return;
                }
            }

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i] is IntelliBoxItem intelliItem &&
                    intelliItem.Text.StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
                {
                    this.SelectedIndex = i;
                    this.TopIndex = i;
                    return;
                }
            }
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
    }
}
