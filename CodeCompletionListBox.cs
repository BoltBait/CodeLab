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
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////
// Intellisense code from http://www.codeproject.com/KB/cs/diy-intellisense.aspx
// which required tons of fixes in order to get it to work properly.
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
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
        private IntelliTypes intelliTypeFilter = IntelliTypes.None;

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
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Method.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Property.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Event.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Field.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Keyword.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Type.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Var.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Class.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Struct.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Enum.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Const.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.EnumItem.png"));
            imageList.Images.Add(new Bitmap(this.GetType(), "Icons.Snippet.png"));
            imageList.Images.Add(new Bitmap(16, 16));
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
            intelliTypeFilter = IntelliTypes.None;
            membersInBox = true;

            MemberInfo[] memberInfo = (isStatic) ?
                type.GetMembers(BindingFlags.Static | BindingFlags.Public) :
                type.GetMembers(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance | BindingFlags.Public);

            for (int i = 0; i < memberInfo.Length; i++)
            {
                if ((!memberInfo[i].ReflectedType.IsVisible && memberInfo[i].DeclaringType?.DeclaringType?.FullName != "PaintDotNet.Effects.UserScript") ||
                    memberInfo[i].ReflectedType.IsSpecialName || memberInfo[i].GetCustomAttribute<ObsoleteAttribute>() != null)
                {
                    continue;
                }

                string returnType = string.Empty;
                string toolTip = string.Empty;

                switch (memberInfo[i].MemberType)
                {
                    case MemberTypes.Method:
                        MethodInfo methodInfo = (MethodInfo)memberInfo[i];
                        if (methodInfo.IsSpecialName)
                        {
                            continue;
                        }

                        string methodParameters = "(";

                        ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                        for (int p = 0; p < parameterInfo.Length; p++)
                        {
                            string parameterType = parameterInfo[p].ParameterType.GetDisplayName();

                            methodParameters += parameterType + " " + parameterInfo[p].Name;

                            if (p < parameterInfo.Length - 1)
                            {
                                methodParameters += ", ";
                            }
                        }

                        methodParameters += ")";

                        returnType = methodInfo.ReturnType.GetDisplayName();
                        toolTip = $"{returnType} - {methodInfo.Name}{methodParameters}\n{methodInfo.MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(methodInfo.Name + methodParameters, methodInfo.Name, toolTip, IntelliTypes.Method));
                        break;
                    case MemberTypes.Property:
                        if (memberInfo[i].Name.Equals("Item", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        string getSet = ((PropertyInfo)memberInfo[i]).GetterSetter();

                        returnType = ((PropertyInfo)memberInfo[i]).PropertyType.GetDisplayName();
                        toolTip = $"{returnType} - {memberInfo[i].Name}{getSet}\n{memberInfo[i].MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(memberInfo[i].Name, memberInfo[i].Name, toolTip, IntelliTypes.Property));
                        break;
                    case MemberTypes.Event:
                        returnType = ((EventInfo)memberInfo[i]).EventHandlerType.GetDisplayName();
                        toolTip = $"{returnType} - {memberInfo[i].Name}\n{memberInfo[i].MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(memberInfo[i].Name, memberInfo[i].Name, toolTip, IntelliTypes.Event));
                        break;
                    case MemberTypes.Field:
                        FieldInfo field = (FieldInfo)memberInfo[i];
                        string fieldTypeName;
                        IntelliTypes fieldType;
                        string fieldValue;

                        if (!field.IsStatic)
                        {
                            fieldTypeName = "Field";
                            fieldType = IntelliTypes.Field;
                            fieldValue = string.Empty;
                        }
                        else if (field.FieldType.IsEnum)
                        {
                            fieldTypeName = "Enum Value";
                            fieldType = IntelliTypes.EnumItem;
                            fieldValue = $" ({(int)field.GetValue(null)})";
                        }
                        else if (field.IsLiteral && !field.IsInitOnly)
                        {
                            fieldTypeName = "Constant";
                            fieldType = IntelliTypes.Constant;
                            fieldValue = $" ({field.GetValue(null)})";
                        }
                        else
                        {
                            fieldTypeName = "Field";
                            fieldType = IntelliTypes.Field;
                            fieldValue = $" ( {field.GetValue(null)} )";
                        }

                        returnType = field.FieldType.GetDisplayName();
                        toolTip = $"{returnType} - {field.Name}{fieldValue}\n{fieldTypeName}";
                        unFilteredItems.Add(new IntelliBoxItem(field.Name, field.Name, toolTip, fieldType));
                        break;
                    case MemberTypes.NestedType:
                        Type nestedType = (Type)memberInfo[i];
                        IntelliTypes intelliType = IntelliTypes.Type;
                        string subType = "Type";

                        if (nestedType.IsEnum)
                        {
                            subType = "enum";
                            intelliType = IntelliTypes.Enum;
                        }
                        else if (nestedType.IsValueType)
                        {
                            subType = "struct";
                            intelliType = IntelliTypes.Struct;
                        }
                        else if (nestedType.IsClass)
                        {
                            subType = "class";
                            intelliType = IntelliTypes.Class;
                        }

                        toolTip = $"{subType} - {nestedType.Name}\nNested Type";
                        unFilteredItems.Add(new IntelliBoxItem(nestedType.Name, nestedType.Name, toolTip, intelliType));
                        break;
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
            intelliTypeFilter = IntelliTypes.None;
            membersInBox = false;

            foreach (string type in Intelli.AutoCompleteTypes.Keys)
            {
                Type t = Intelli.AutoCompleteTypes[type];

                string baseType = "Type";
                string name = type;
                string code = type;
                IntelliTypes icon = IntelliTypes.Type;

                if (t.IsEnum)
                {
                    baseType = "enum";
                    icon = IntelliTypes.Enum;
                }
                else if (t.IsValueType)
                {
                    baseType = "struct";
                    icon = IntelliTypes.Struct;
                }
                else if (t.IsClass)
                {
                    baseType = "class";
                    icon = IntelliTypes.Class;
                }
                else if (t.IsInterface)
                {
                    // Disabled simply because I don't think they're useful in the context of CodeLab
                    //baseType = "interface";
                    //icon = IntelliTypes.Interface;
                    continue;
                }

                if (t.IsGenericType)
                {
                    string shortName = System.Text.RegularExpressions.Regex.Replace(t.Name, @"`\d", string.Empty);
                    if (type.StartsWith(shortName, StringComparison.OrdinalIgnoreCase))
                    {
                        Type[] generics = t.GetGenericArguments();
                        string args = string.Empty;
                        for (int i = 0; i < generics.Length; i++)
                        {
                            args += generics[i].Name + (i == generics.Length - 1 ? string.Empty : ", ");
                        }

                        name = $"{shortName}<{args}>";
                        code = $"{shortName}<>";
                    }
                }

                string toolTip = $"{baseType} - {t.Namespace}.{name}\nType";
                unFilteredItems.Add(new IntelliBoxItem(name, code, toolTip, icon));
            }

            foreach (string type in Intelli.UserDefinedTypes.Keys)
            {
                Type t = Intelli.UserDefinedTypes[type];

                string baseType = "Type";
                string name = type;
                string code = type;
                IntelliTypes icon = IntelliTypes.Type;

                if (t.IsEnum)
                {
                    baseType = "enum";
                    icon = IntelliTypes.Enum;
                }
                else if (t.IsValueType)
                {
                    baseType = "struct";
                    icon = IntelliTypes.Struct;
                }
                else if (t.IsClass)
                {
                    baseType = "class";
                    icon = IntelliTypes.Class;
                }
                else if (t.IsInterface)
                {
                    // Disabled simply because I don't think they're useful in the context of CodeLab
                    //baseType = "interface";
                    //icon = IntelliTypes.Interface;
                    continue;
                }

                if (t.IsGenericType)
                {
                    string shortName = System.Text.RegularExpressions.Regex.Replace(t.Name, @"`\d", string.Empty);
                    if (type.StartsWith(shortName, StringComparison.OrdinalIgnoreCase))
                    {
                        Type[] generics = t.GetGenericArguments();
                        string args = string.Empty;
                        for (int i = 0; i < generics.Length; i++)
                        {
                            args += generics[i].Name + (i == generics.Length - 1 ? string.Empty : ", ");
                        }

                        name = $"{shortName}<{args}>";
                        code = $"{shortName}<>";
                    }
                }

                string toolTip = $"{baseType} - {t.DeclaringType.Name}.{name}\nType";
                unFilteredItems.Add(new IntelliBoxItem(name, code, toolTip, icon));
            }

            foreach (string var in Intelli.Variables.Keys)
            {
                string type = Intelli.Variables[var].GetDisplayName();
                string toolTip = $"{type} - {var}\nLocal Variable";
                unFilteredItems.Add(new IntelliBoxItem(var, var, toolTip, IntelliTypes.Variable));
            }

            foreach (string para in Intelli.Parameters.Keys)
            {
                string type = Intelli.Parameters[para].GetDisplayName();
                string toolTip = $"{type} - {para}\nParameter";
                unFilteredItems.Add(new IntelliBoxItem(para, para, toolTip, IntelliTypes.Variable)); // use the var icon
            }

            foreach (string key in Intelli.Keywords)
            {
                string toolTip = $"{key}\nKeyword";
                unFilteredItems.Add(new IntelliBoxItem(key, key, toolTip, IntelliTypes.Keyword));
            }

            foreach (string snip in Intelli.Snippets.Keys)
            {
                string toolTip = $"{snip}\nSnippet - Tab Twice";
                unFilteredItems.Add(new IntelliBoxItem(snip, snip, toolTip, IntelliTypes.Snippet));
            }

            MemberInfo[] memberInfo = Intelli.UserScript.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < memberInfo.Length; i++)
            {
                if (memberInfo[i].GetCustomAttribute<ObsoleteAttribute>() != null)
                {
                    continue;
                }

                string returnType = string.Empty;
                string toolTip = string.Empty;

                switch (memberInfo[i].MemberType)
                {
                    case MemberTypes.Method:
                        MethodInfo methodInfo = (MethodInfo)memberInfo[i];
                        if (methodInfo.IsSpecialName || methodInfo.DeclaringType != Intelli.UserScript ||
                            methodInfo.Name.Equals("Render", StringComparison.Ordinal) || methodInfo.Name.Equals("PreRender", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        string methodParameters = "(";

                        ParameterInfo[] parameterInfo = methodInfo.GetParameters();
                        for (int p = 0; p < parameterInfo.Length; p++)
                        {
                            string parameterType = parameterInfo[p].ParameterType.GetDisplayName();

                            methodParameters += parameterType + " " + parameterInfo[p].Name;

                            if (p < parameterInfo.Length - 1)
                            {
                                methodParameters += ", ";
                            }
                        }

                        methodParameters += ")";

                        returnType = methodInfo.ReturnType.GetDisplayName();
                        toolTip = $"{returnType} - {methodInfo.Name}{methodParameters}\n{methodInfo.MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(methodInfo.Name + methodParameters, methodInfo.Name, toolTip, IntelliTypes.Method));
                        break;
                    case MemberTypes.Property:
                        if (memberInfo[i].Name.Equals("SetRenderInfoCalled", StringComparison.Ordinal) || memberInfo[i].Name.Equals("__DebugMsgs", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        string getSet = ((PropertyInfo)memberInfo[i]).GetterSetter();

                        returnType = ((PropertyInfo)memberInfo[i]).PropertyType.GetDisplayName();
                        toolTip = $"{returnType} - {memberInfo[i].Name}{getSet}\n{memberInfo[i].MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(memberInfo[i].Name, memberInfo[i].Name, toolTip, IntelliTypes.Property));
                        break;
                    case MemberTypes.Event:
                        returnType = ((EventInfo)memberInfo[i]).EventHandlerType.GetDisplayName();
                        toolTip = $"{returnType} - {memberInfo[i].Name}\n{memberInfo[i].MemberType}";
                        unFilteredItems.Add(new IntelliBoxItem(memberInfo[i].Name, memberInfo[i].Name, toolTip, IntelliTypes.Event));
                        break;
                    case MemberTypes.Field:
                        if (memberInfo[i].Name.Equals("RandomNumber", StringComparison.Ordinal) || memberInfo[i].Name.Equals("instanceSeed", StringComparison.Ordinal) ||
                            memberInfo[i].Name.Equals("__listener", StringComparison.Ordinal) || memberInfo[i].Name.Equals("__debugWriter", StringComparison.Ordinal))
                        {
                            continue;
                        }

                        FieldInfo field = (FieldInfo)memberInfo[i];
                        string fieldTypeName;
                        IntelliTypes fieldType;
                        string fieldValue;

                        if (!field.IsStatic)
                        {
                            fieldTypeName = "Field";
                            fieldType = IntelliTypes.Field;
                            fieldValue = string.Empty;
                        }
                        else if (field.FieldType.IsEnum)
                        {
                            fieldTypeName = "Enum Value";
                            fieldType = IntelliTypes.EnumItem;
                            fieldValue = $" ({(int)field.GetValue(null)})";
                        }
                        else if (field.IsLiteral && !field.IsInitOnly)
                        {
                            fieldTypeName = "Constant";
                            fieldType = IntelliTypes.Constant;
                            fieldValue = $" ({field.GetValue(null)})";
                        }
                        else
                        {
                            fieldTypeName = "Field";
                            fieldType = IntelliTypes.Field;
                            fieldValue = $" ( {field.GetValue(null)} )";
                        }

                        returnType = field.FieldType.GetDisplayName();
                        toolTip = $"{returnType} - {memberInfo[i].Name}{fieldValue}\n{fieldTypeName}";
                        unFilteredItems.Add(new IntelliBoxItem(memberInfo[i].Name, memberInfo[i].Name, toolTip, fieldType));
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
            intelliTypeFilter = IntelliTypes.None;
            membersInBox = false;

            if (!type.IsEnum && type.IsValueType)
            {
                unFilteredItems.Add(new IntelliBoxItem($"{type.Name}()", string.Empty, $"{type.Name}()", IntelliTypes.Constructor));
            }

            ConstructorInfo[] constructors = type.GetConstructors();
            for (int i = 0; i < constructors.Length; i++)
            {
                ParameterInfo[] parameters = constructors[i].GetParameters();
                string ctorString = string.Empty;
                for (int j = 0; j < parameters.Length; j++)
                {
                    ctorString += $"{parameters[j].ParameterType.GetDisplayName()} {parameters[j].Name}{(j == parameters.Length - 1 ? string.Empty : ", ")}";
                }

                string toolTip = $"{type.Name}({ctorString})";
                unFilteredItems.Add(new IntelliBoxItem(toolTip, string.Empty, toolTip, IntelliTypes.Constructor));
            }

            this.Items.AddRange(unFilteredItems.ToArray());
            if (unFilteredItems.Count > 0)
            {
                this.SelectedIndex = 0;
            }
        }

        internal void Filter(string contains)
        {
            stringFilter = contains;

            Filter();
        }

        internal void Filter(IntelliTypes intelliTypes)
        {
            intelliTypeFilter = (intelliTypes == intelliTypeFilter) ? IntelliTypes.None : intelliTypes;

            Filter();
        }

        private void Filter()
        {
            List<IntelliBoxItem> matches = new List<IntelliBoxItem>();
            for (int i = 0; i < unFilteredItems.Count; i++)
            {
                if (intelliTypeFilter != IntelliTypes.None && unFilteredItems[i].ImageIndex != (int)intelliTypeFilter)
                {
                    continue;
                }

                string itemName = unFilteredItems[i].ToString();
                if (!itemName.Contains(stringFilter, StringComparison.OrdinalIgnoreCase) && !itemName.GetInitials().Contains(stringFilter, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matches.Add(unFilteredItems[i]);
            }

            if (intelliTypeFilter == IntelliTypes.None && matches.Count == 0)
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
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (Items[e.Index] is IntelliBoxItem item)
            {
                if (item.ImageIndex != -1)
                {
                    using (SolidBrush iconBg = new SolidBrush(this.BackColor))
                        e.Graphics.FillRectangle(iconBg, e.Bounds.Left, e.Bounds.Top, imageList.ImageSize.Width + 1, imageList.ImageSize.Height);
                    imageList.Draw(e.Graphics, e.Bounds.Left, e.Bounds.Top, item.ImageIndex);
                    TextRenderer.DrawText(e.Graphics, item.Text, e.Font, new Point(e.Bounds.Left + imageList.ImageSize.Width, e.Bounds.Top), e.ForeColor);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, item.Text, e.Font, new Point(e.Bounds.Left, e.Bounds.Top), e.ForeColor);
                }
            }
            else
            {
                if (e.Index != -1)
                {
                    TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, new Point(e.Bounds.Left, e.Bounds.Top), e.ForeColor);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, Text, e.Font, new Point(e.Bounds.Left, e.Bounds.Top), e.ForeColor);
                }
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
                else if (item.ImageIndex != (int)IntelliTypes.Keyword && item.ImageIndex != (int)IntelliTypes.Snippet)
                {
                    this.LastUsedNonMember = item;
                }
            }
        }
    }

    internal class IntelliBoxItem : IComparable, IEquatable<IntelliBoxItem>
    {
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj is IntelliBoxItem other)
            {
                return (string.Compare(this.Text, other.Text, StringComparison.OrdinalIgnoreCase));
            }

            return 0;
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

        internal static readonly IntelliBoxItem Empty = new IntelliBoxItem(string.Empty, string.Empty, string.Empty, IntelliTypes.None);

        internal string Text { get; }
        internal string Code { get; }
        internal string ToolTip { get; }
        internal int ImageIndex { get; }

        internal IntelliBoxItem(string text, string code, string toolTip, IntelliTypes intelliType)
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
