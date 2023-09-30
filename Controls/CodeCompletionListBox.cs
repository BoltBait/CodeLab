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

using PaintDotNet;
using PaintDotNet.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal sealed class IntelliBox : UserControl
    {
        private bool listBoxMouseOver;
        private bool toolstripMouseOver;
        private bool drawSelectionOutline;
        private bool isStaticColorType;
        private readonly IntelliTip itemToolTip = new IntelliTip();
        private readonly IntelliTip filterToolTip = new IntelliTip();
        private readonly List<IntelliBoxItem> unFilteredItems = new List<IntelliBoxItem>();
        private IntelliBoxItem LastUsedMember = IntelliBoxItem.Empty;
        private IntelliBoxItem LastUsedNonMember = IntelliBoxItem.Empty;
        private IntelliBoxContents intelliBoxContents;
        private string stringFilter = string.Empty;
        private readonly ICollection<IntelliType> enumFilters = new List<IntelliType>();
        private readonly ListBox listBox = new ListBox();
        private readonly ToolStrip toolStrip = new ToolStrip();
        private const int visibleItems = 9;

        internal string AutoCompleteCode => listBox.SelectedIndex >= 0 ? listBox.SelectedItem.ToString() : string.Empty;
        internal bool MouseOver => listBoxMouseOver || toolstripMouseOver;
        internal bool AutoComplete => !drawSelectionOutline;
        internal int IconWidth => IconSize.Width + 2;
        internal bool ExtraSpace => (intelliBoxContents != IntelliBoxContents.NonMembers && intelliBoxContents != IntelliBoxContents.Constructors);
        internal bool IsEmpty => this.listBox.Items.Count == 0;

        private static readonly Size IconSize = UIUtil.ScaleSize(16, 16);
        private static readonly IReadOnlyList<Image> ItemIcons = new Image[]
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
        private static readonly IEnumerable<char> confirmationChars = new[]
        {
            '{', '}', '(', ')', '[', ']', '<', '>',
            '!', '@', '#', '%', '^', '&', '|', '~',
            '*', '/', '+', '-', '=', '?', ',', '\\'
        };

        internal IntelliBox()
        {
            FilterButton[] filterButtons = Enum.GetValues<IntelliType>()
                .Where(i => i != IntelliType.None && i != IntelliType.Type && i != IntelliType.Parameter)
                .Select(i =>
                {
                    FilterButton filterButton = new FilterButton(i);
                    filterButton.Click += FilterButton_Click;
                    filterButton.MouseHover += FilterButton_MouseHover;
                    filterButton.MouseLeave += FilterButton_MouseLeave;

                    return filterButton;
                })
                .ToArray();

            toolStrip.Items.AddRange(filterButtons);
            toolStrip.Dock = DockStyle.Bottom;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.ShowItemToolTips = false;
            toolStrip.MouseEnter += ToolStrip_MouseEnter;
            toolStrip.MouseLeave += ToolStrip_MouseLeave;

            listBox.Dock = DockStyle.Fill;
            listBox.DrawMode = DrawMode.OwnerDrawFixed;
            listBox.ItemHeight = UIUtil.Scale(16);
            listBox.BorderStyle = BorderStyle.None;
            listBox.DrawItem += ListBox_DrawItem;
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            listBox.DoubleClick += ListBox_DoubleClick;
            listBox.Click += ListBox_Click;
            listBox.MouseEnter += ListBox_MouseEnter;
            listBox.MouseLeave += ListBox_MouseLeave;

            this.Controls.Add(listBox);
            this.Controls.Add(toolStrip);
            this.Cursor = Cursors.Default;
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        internal void SelectFirst()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else
                {
                    this.listBox.SelectedIndex = 0;
                }
            }
        }

        internal void SelectLast()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else
                {
                    this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
                }
            }
        }

        internal void SelectNext()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else if (this.listBox.SelectedIndex < this.listBox.Items.Count - 1)
                {
                    this.listBox.SelectedIndex++;
                }
            }
        }

        internal void SelectPrev()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else if (this.listBox.SelectedIndex > 0)
                {
                    this.listBox.SelectedIndex--;
                }
            }
        }

        internal void PageDown()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else if (this.listBox.SelectedIndex < this.listBox.Items.Count - visibleItems)
                {
                    this.listBox.SelectedIndex += visibleItems;
                }
                else
                {
                    this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
                }
            }
        }

        internal void PageUp()
        {
            if (this.listBox.Items.Count > 0)
            {
                if (this.drawSelectionOutline && this.listBox.SelectedIndex > -1)
                {
                    this.drawSelectionOutline = false;
                    this.listBox.InvalidateSelectedIndex();
                }
                else if (this.listBox.SelectedIndex > visibleItems)
                {
                    this.listBox.SelectedIndex -= visibleItems;
                }
                else
                {
                    this.listBox.SelectedIndex = 0;
                }
            }
        }

        internal void ScrollItems(int delta)
        {
            if (this.listBoxMouseOver && this.listBox.Items.Count > 0)
            {
                int newTopIndex = this.listBox.TopIndex - Math.Sign(delta) * SystemInformation.MouseWheelScrollLines;
                this.listBox.TopIndex = Math.Clamp(newTopIndex, 0, this.listBox.Items.Count - 1);
            }
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            this.itemToolTip.Hide(this);

            base.OnLocationChanged(e);
        }

        private void ListBox_MouseEnter(object sender, EventArgs e)
        {
            this.listBoxMouseOver = true;
        }

        private void ToolStrip_MouseLeave(object sender, EventArgs e)
        {
            this.toolstripMouseOver = false;
        }

        private void ListBox_MouseLeave(object sender, EventArgs e)
        {
            this.listBoxMouseOver = false;
        }

        private void ToolStrip_MouseEnter(object sender, EventArgs e)
        {
            this.toolstripMouseOver = true;
        }

        private void ListBox_DoubleClick(object sender, EventArgs e)
        {
            ((CodeTextBox)Parent).ConfirmIntelliBox();
        }

        private void ListBox_Click(object sender, EventArgs e)
        {
            Parent.Focus();
        }

        private void FilterButton_MouseLeave(object sender, EventArgs e)
        {
            this.filterToolTip.Hide(this);
        }

        private void FilterButton_MouseHover(object sender, EventArgs e)
        {
            if (sender is FilterButton filterButton)
            {
                Cursor currentCursor = Cursor.Current;
                if (currentCursor is not null)
                {
                    Point cursorLocation = Cursor.Position;
                    cursorLocation.X += currentCursor.HotSpot.X + 2;
                    cursorLocation.Y += Cursor.Size.Height - currentCursor.HotSpot.Y;

                    this.filterToolTip.Show(filterButton.ToolTipText, this, PointToClient(cursorLocation));
                }
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (sender is FilterButton filterButton)
            {
                Filter(filterButton.IntelliType);
            }
        }

        /// <summary>
        /// Fills the iBox with members of the given Type.
        /// </summary>
        internal void PopulateMembers(Type type, bool isStatic)
        {
            Clear();
            this.isStaticColorType = isStatic && (type == typeof(ColorBgra) || type == typeof(Color) || type == typeof(SrgbColors) || type == typeof(LinearColors));
            this.intelliBoxContents = IntelliBoxContents.Members;

            BindingFlags bindingFlags = isStatic ?
                BindingFlags.Static | BindingFlags.Public :
                BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance | BindingFlags.Public;

            MemberInfo[] members = type.GetMembers(bindingFlags);
            if (type.IsInterface)
            {
                IEnumerable<MemberInfo> iMembers = type.GetInterfaces().SelectMany(x => x.GetMembers(bindingFlags));
                members = members.UnionBy(iMembers, x => x.Name).ToArray();
            }

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
            this.listBox.Items.AddRange(unFilteredItems.ToArray());

            if (this.listBox.Items.Contains(this.LastUsedMember))
            {
                this.listBox.SelectedItem = this.LastUsedMember;
            }
            else if (this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }

            IReadOnlyCollection<IntelliType> matchingTypes = unFilteredItems.Select(item => item.IntelliType).Distinct().ToArray();
            SetFilterButtonVisibility(matchingTypes);
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

                IEnumerable<IGrouping<MemberTypes, MemberInfo>> userScriptMembers = Intelli.UserScript
                    .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => !m.IsObsolete() && !m.IsCompilerGenerated())
                    .GroupBy(m => m.MemberType);

                foreach (IGrouping<MemberTypes, MemberInfo> group in userScriptMembers)
                {
                    switch (group.Key)
                    {
                        case MemberTypes.Method:
                            group.OfType<MethodInfo>()
                                .Where(m =>
                                {
                                    return !(m.IsSpecialName ||
                                        m.IsVirtual ||
                                        !m.IsVisibleToReflectedType() ||
                                        m.Name.Equals("Render", StringComparison.Ordinal) ||
                                        m.Name.Equals("PreRender", StringComparison.Ordinal) ||
                                        m.Name.Equals("SetRenderInfo", StringComparison.Ordinal));
                                })
                                .ForEach(m => AddMethod(m, false));

                            break;
                        case MemberTypes.Property:
                            group.OfType<PropertyInfo>()
                                .Where(p =>
                                {
                                    return !(!p.IsVisibleToReflectedType() ||
                                        p.Name.Equals("__DebugMsgs", StringComparison.Ordinal) ||
                                        p.IsIndexer());
                                })
                                .DistinctBy(p => p.Name)
                                .ForEach(p => AddProperty(p));

                            break;
                        case MemberTypes.Event:
                            group.OfType<EventInfo>()
                                .ForEach(e => AddEvent(e));

                            break;
                        case MemberTypes.Field:
                            group.OfType<FieldInfo>()
                                .Where(f =>
                                {
                                    return !(f.Name.Equals("RandomNumber", StringComparison.Ordinal) ||
                                        f.Name.Equals("instanceSeed", StringComparison.Ordinal) ||
                                        f.Name.Equals("__listener", StringComparison.Ordinal) ||
                                        f.Name.Equals("__debugWriter", StringComparison.Ordinal));
                                })
                                .ForEach(f => AddField(f));

                            break;
                    }
                }
            }

            IReadOnlyCollection<IntelliType> matchingTypes = unFilteredItems.Select(item => item.IntelliType).Distinct().ToArray();
            SetFilterButtonVisibility(matchingTypes);

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

            this.listBox.Items.AddRange(unFilteredItems.ToArray());
            if (unFilteredItems.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }

            SetFilterButtonVisibility(Array.Empty<IntelliType>());
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

            unFilteredItems.AddRange(GenerateSuggestedNames(typeName)
                .Select(str =>
                    {
                        string suggestion = plural ? str.MakePlural() : str;
                        return new IntelliBoxItem(suggestion, suggestion, "(Suggested name)", IntelliType.Variable);
                    })
                );

            this.listBox.Items.AddRange(unFilteredItems.ToArray());
            this.listBox.SelectedIndex = 0;

            SetFilterButtonVisibility(Array.Empty<IntelliType>());
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
            this.listBox.Items.AddRange(unFilteredItems.ToArray());

            if (this.listBox.Items.Contains(this.LastUsedNonMember))
            {
                this.listBox.SelectedItem = this.LastUsedNonMember;
            }
            else if (this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }

            SetFilterButtonVisibility(Array.Empty<IntelliType>());
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

            this.listBox.Items.AddRange(unFilteredItems.ToArray());

            if (this.listBox.Items.Contains(this.LastUsedMember))
            {
                this.listBox.SelectedItem = this.LastUsedMember;
            }
            else if (this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }

            SetFilterButtonVisibility(Array.Empty<IntelliType>());
        }

        private void Clear()
        {
            this.listBox.Items.Clear();
            this.unFilteredItems.Clear();
            this.stringFilter = string.Empty;
            this.enumFilters.Clear();
            this.isStaticColorType = false;
        }

        private void AddMethod(MethodInfo methodInfo, bool isExtension)
        {
            string returnType = methodInfo.ReturnType.GetDisplayName();
            string nullable = methodInfo.IsNullable() ? "?" : string.Empty;
            string byRef = methodInfo.ReturnType.IsByRef ? "ref " : string.Empty;
            string methodParameters = $"({methodInfo.Params()})";
            string genericArgs = string.Empty;
            string genericConstraints = string.Empty;
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
                        genericConstraints = "\r\n    " + constraints;
                    }
                }
            }

            string toolTip = $"{byRef}{returnType}{nullable} - {methodInfo.Name}{genericArgs}{methodParameters}{genericConstraints}\n{ext}{methodInfo.MemberType}";
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

        private static IEnumerable<string> GenerateSuggestedNames(string str)
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

        internal void Filter(IntelliType intelliType)
        {
            IEnumerable<FilterButton> filterButtons = this.toolStrip.Items.OfType<FilterButton>();
            FilterButton filterButton = filterButtons.FirstOrDefault(b => b.Visible && b.IntelliType == intelliType);

            if (filterButton is null)
            {
                return;
            }

            filterButton.Checked = !filterButton.Checked;

            IEnumerable<IntelliType> matchingTypes = listBox.Items.OfType<IntelliBoxItem>().Select(i => i.IntelliType).Distinct().ToArray();
            if (filterButtons.Any(x => x.Checked))
            {
                IEnumerable<IntelliType> checkedTypes = filterButtons.Where(x => x.Checked).Select(x => x.IntelliType).Intersect(matchingTypes);
                SetFilterButtonAppearance(checkedTypes);
            }
            else
            {
                SetFilterButtonAppearance(matchingTypes);
            }

            if (intelliType == IntelliType.None)
            {
                enumFilters.Clear();
            }
            else if (enumFilters.Contains(intelliType))
            {
                enumFilters.Remove(intelliType);
            }
            else
            {
                enumFilters.Add(intelliType);
            }

            Filter();
        }

        private void Filter()
        {
            List<IntelliBoxItem> matches = new List<IntelliBoxItem>();
            foreach (IntelliBoxItem item in unFilteredItems)
            {
                if (enumFilters.Count > 0 && !enumFilters.Contains(item.IntelliType))
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

            IEnumerable<IntelliType> matchingTypes = matches.Select(item => item.IntelliType).Distinct().ToArray();
            SetFilterButtonAppearance(matchingTypes);

            if (enumFilters.Count == 0 && matches.Count == 0)
            {
                drawSelectionOutline = true;
                listBox.InvalidateSelectedIndex();
                return;
            }

            this.listBox.Items.Clear();

            if (matches.Count == 0)
            {
                itemToolTip.Hide(this);
                drawSelectionOutline = true;
                return;
            }

            drawSelectionOutline = false;
            this.listBox.Items.AddRange(matches.ToArray());

            if (stringFilter.Length == 0)
            {
                if (this.listBox.Items.Contains(LastUsedMember))
                {
                    this.listBox.SelectedItem = LastUsedMember;
                }
                else if (this.listBox.Items.Contains(LastUsedNonMember))
                {
                    this.listBox.SelectedItem = LastUsedNonMember;
                }
                else
                {
                    this.listBox.SelectedIndex = 0;
                }

                return;
            }

            int indexToSelect = this.listBox.FindStringExact(stringFilter, true);
            if (indexToSelect == -1)
            {
                if (this.listBox.Items.Contains(LastUsedMember) && LastUsedMember.ToString().StartsWith(stringFilter, StringComparison.Ordinal))
                {
                    indexToSelect = this.listBox.FindStringExact(LastUsedMember.ToString(), false);
                }
                else if (this.listBox.Items.Contains(LastUsedNonMember) && LastUsedNonMember.ToString().StartsWith(stringFilter, StringComparison.Ordinal))
                {
                    indexToSelect = this.listBox.FindStringExact(LastUsedNonMember.ToString(), false);
                }

                if (indexToSelect == -1)
                {
                    indexToSelect = this.listBox.FindString(stringFilter, true);
                    if (indexToSelect == -1)
                    {
                        indexToSelect = 0;
                    }
                }
            }

            this.listBox.SelectedIndex = indexToSelect;
            this.listBox.TopIndex = indexToSelect;
        }

        private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                return;
            }

            if (this.listBox.Items[e.Index] is not IntelliBoxItem item)
            {
                e.DrawBackground();
                TextRenderer.DrawText(e.Graphics, this.listBox.Items[e.Index].ToString(), e.Font, e.Bounds, e.ForeColor, TextFormatFlags.Default);
                return;
            }

            Rectangle textRect = Rectangle.FromLTRB(e.Bounds.Left + IconSize.Width + 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom - 1);
            bool outline = e.State == DrawItemState.Selected && this.drawSelectionOutline;

            if (!outline)
            {
                e.DrawBackground();
                using (SolidBrush iconBg = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(iconBg, e.Bounds.Left, e.Bounds.Top, IconSize.Width + 1, IconSize.Height);
                }
            }

            e.Graphics.DrawImage(ItemIcons[item.ImageIndex], e.Bounds.Left, e.Bounds.Top, IconSize.Width, IconSize.Height);

            Color textColor = outline ? listBox.ForeColor : e.ForeColor;
            TextRenderer.DrawText(e.Graphics, item.Text, e.Font, textRect, textColor, TextFormatFlags.EndEllipsis);

            if (isStaticColorType &&
                item.IntelliType == IntelliType.Property &&
                Enum.TryParse(item.Text, false, out KnownColor knownColor))
            {
                const int padding = 2;
                int swatchHeight = e.Bounds.Height - 4;

                Rectangle rect = Rectangle.FromLTRB(
                    e.Bounds.Right - swatchHeight - padding,
                    e.Bounds.Top + padding,
                    e.Bounds.Right - padding - 1,
                    e.Bounds.Bottom - padding - 1);

                Rectangle innerRect = Rectangle.FromLTRB(
                    rect.Left + 1,
                    rect.Top + 1,
                    rect.Right,
                    rect.Bottom);

                using SolidBrush brush = new SolidBrush(Color.FromKnownColor(knownColor));
                e.Graphics.FillRectangle(brush, innerRect);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }

            if (outline)
            {
                e.Graphics.DrawRectangle(SystemPens.Highlight, textRect);
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.drawSelectionOutline)
            {
                this.drawSelectionOutline = false;
                this.listBox.InvalidateSelectedIndex();
            }

            if (this.listBox.Visible && this.listBox.SelectedItem is IntelliBoxItem item)
            {
                itemToolTip.Show(item.ToolTip, this, this.Width, 0);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible)
            {
                itemToolTip.Hide(this);
            }
            else if (this.listBox.SelectedItem is IntelliBoxItem item)
            {
                itemToolTip.Show(item.ToolTip, this, this.Width, 0);
            }
            else
            {
                itemToolTip.Hide(this);
            }

            base.OnVisibleChanged(e);
        }

        private void SetFilterButtonVisibility(IReadOnlyCollection<IntelliType> intelliTypes)
        {
            if (intelliTypes.Count > 0)
            {
                this.toolStrip.Visible = true;

                foreach (FilterButton filterButton in this.toolStrip.Items)
                {
                    filterButton.Checked = false;
                    filterButton.Active = true;
                    filterButton.Visible = intelliTypes.Contains(filterButton.IntelliType);
                }

                int totalItemWidth = (this.toolStrip.Items[0].Width + this.toolStrip.Items[0].Margin.Left) * intelliTypes.Count;
                int newWidth = Math.Max(UIUtil.Scale(300), totalItemWidth);
                this.ClientSize = new Size(newWidth, toolStrip.Height + listBox.ItemHeight * visibleItems);
            }
            else
            {
                this.toolStrip.Visible = false;
                this.ClientSize = new Size(UIUtil.Scale(300), listBox.ItemHeight * visibleItems);
            }
        }

        private void SetFilterButtonAppearance(IEnumerable<IntelliType> intelliTypes)
        {
            foreach (FilterButton filterButton in this.toolStrip.Items)
            {
                filterButton.Active = intelliTypes.Contains(filterButton.IntelliType);
            }
        }

        internal void UpdateTheme(Color toolTipFore, Color toolTipBack)
        {
            this.ForeColor = PdnTheme.ForeColor;
            this.BackColor = PdnTheme.BackColor;
            this.toolStrip.Renderer = PdnTheme.Renderer;
            this.itemToolTip.UpdateTheme(toolTipFore, toolTipBack);
            this.filterToolTip.UpdateTheme(toolTipFore, toolTipBack);
            this.listBox.ForeColor = PdnTheme.ForeColor;
            this.listBox.BackColor = PdnTheme.BackColor;
            this.listBox.EnableUxThemeDarkMode(PdnTheme.Theme == Theme.Dark);
        }

        internal void SaveUsedItem()
        {
            if (this.listBox.SelectedItem is IntelliBoxItem item)
            {
                if (this.intelliBoxContents == IntelliBoxContents.Members || this.intelliBoxContents == IntelliBoxContents.XamlAttributes)
                {
                    this.LastUsedMember = item;
                }
                else if ((this.intelliBoxContents == IntelliBoxContents.NonMembers || this.intelliBoxContents == IntelliBoxContents.XamlTags) &&
                    item.IntelliType != IntelliType.Keyword && item.IntelliType != IntelliType.Snippet)
                {
                    this.LastUsedNonMember = item;
                }
            }
        }

        internal void FindAndSelect(string itemName)
        {
            int itemIndex = this.listBox.FindStringExact(itemName, true);
            if (itemIndex == -1)
            {
                itemIndex = this.listBox.FindString(itemName, true);
                if (itemIndex == -1)
                {
                    return;
                }
            }

            this.listBox.SelectedIndex = itemIndex;
            this.listBox.TopIndex = itemIndex;
        }

        internal static bool IsConfirmationChar(char c)
        {
            return confirmationChars.Contains(c);
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
                    return Equals(other);
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
            internal IntelliType IntelliType { get; }

            internal IntelliBoxItem(string text, string code, string toolTip, IntelliType intelliType)
            {
                this.Text = text;
                this.Code = code;
                this.ToolTip = toolTip;
                this.ImageIndex = (int)intelliType;
                this.IntelliType = intelliType;
            }

            public override string ToString()
            {
                return Code;
            }
        }

        private sealed class FilterButton : ScaledToolStripButton
        {
            private bool active = true;

            internal IntelliType IntelliType { get; }

            internal bool Active
            {
                get
                {
                    return this.active;
                }
                set
                {
                    if (value != this.active)
                    {
                        this.active = value;
                        this.Image = value
                            ? ItemIcons[(int)this.IntelliType]
                            : UIUtil.CreateDisabledImage(ItemIcons[(int)this.IntelliType]);
                    }
                }
            }

            internal FilterButton(IntelliType intelliType)
            {
                this.IntelliType = intelliType;
                this.Image = ItemIcons[(int)this.IntelliType];
                this.ToolTipText = $"Show only {intelliType.ToString().MakePlural()}{GetHotKey(intelliType)}";
                this.Margin = new Padding(UIUtil.Scale(2));
            }

            private static string GetHotKey(IntelliType intelliType)
            {
                return intelliType switch
                {
                    IntelliType.Method => " (Alt+M)",
                    IntelliType.Property => " (Alt+P)",
                    IntelliType.Event => " (Alt+V)",
                    IntelliType.Field => " (Alt+F)",
                    IntelliType.Keyword => " (Alt+K)",
                    IntelliType.Variable => " (Alt+L)",
                    IntelliType.Class => " (Alt+C)",
                    IntelliType.Struct => " (Alt+S)",
                    IntelliType.Enum => " (Alt+E)",
                    IntelliType.Constant => " (Alt+O)",
                    IntelliType.Snippet => " (Alt+T)",
                    IntelliType.Interface => " (Alt+I)",
                    IntelliType.Delegate => " (Alt+D)",
                    _ => string.Empty,
                };
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
