/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI.ViewManagement;

namespace PdnCodeLab
{
    public enum Theme
    {
        Auto,
        Light,
        Dark
    }

    [Flags]
    internal enum ItemSelectionFlags
    {
        //None = 0,
        Fill = 1,
        Outline = 2,
        AccentMark = 4,
        AccentOutline = 8
    }

    internal interface IIntelliTipHost
    {
        void ThemeToolTip(Color toolTipFore, Color toolTipBack);
    }

    internal interface IToolTipHost
    {
        void ThemeToolTip();
    }

    internal static class ThemeUtil
    {
        private static bool isAppThemeDark;
        private static Color originalForeColor;
        private static Color originalBackColor;

        private static Theme currentTheme;
        private static Color foreColor;
        private static Color backColor;
        private static ThemeRenderer themeRenderer;
        private static TabRenderer tabRenderer;
        private static Color accentColor;
        private static readonly UISettings uiSettings = new UISettings();

        internal static void Initialize(bool PdnDarkTheme, Color foreColor, Color backColor, Theme userTheme)
        {
            isAppThemeDark = PdnDarkTheme;
            originalForeColor = foreColor;
            originalBackColor = backColor;
            SetTheme(userTheme);
        }

        private static void SetTheme(Theme value)
        {
            switch (value)
            {
                case Theme.Auto:
                    foreColor = originalForeColor;
                    backColor = originalBackColor;
                    break;
                case Theme.Dark:
                    foreColor = Color.White;
                    backColor = Color.FromArgb(40, 40, 40);
                    break;
                case Theme.Light:
                    foreColor = Color.Black;
                    backColor = Color.FromArgb(240, 240, 240);
                    break;
            }

            currentTheme = value;
            themeRenderer = null;
            tabRenderer = null;
        }

        private static ThemeRenderer ToolStripRenderer => themeRenderer ??= new ThemeRenderer();
        private static TabRenderer TabBarRenderer => tabRenderer ??= new TabRenderer();

        private static Color AccentColor
        {
            get
            {
                if (accentColor.IsEmpty)
                {
                    accentColor = uiSettings.GetColorValue(UIColorType.AccentLight2).ToGdiColor();
                }

                return accentColor;
            }
        }

        internal static void UpdateTheme(this Form form, Theme theme)
        {
            SetTheme(theme);
            UpdateTheme(form);
        }

        internal static void UpdateTheme(this Form form)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            bool effectiveDarkMode = currentTheme == Theme.Dark || (currentTheme == Theme.Auto && isAppThemeDark);

            form.ForeColor = foreColor;
            form.BackColor = backColor;

            if (form is IToolTipHost toolTipHost)
            {
                toolTipHost.ThemeToolTip();
            }

            foreach (Control c in form.Descendants<Control>())
            {
                if (c.Parent is TabPage || c.Parent is TabControl)
                {
                    continue;
                }

                switch (c)
                {
                    case ToolStrip toolStrip:
                        toolStrip.Renderer = (toolStrip.Parent is TabStrip)
                            ? TabBarRenderer
                            : ToolStripRenderer;

                        foreach (ToolStrip ts in toolStrip.Descendants())
                        {
                            ts.GetToolTip().UpdateTheme();

                            if (ts is ToolStripDropDown dropDown)
                            {
                                dropDown.Opening -= AddMenuPadding;
                                dropDown.Opening += AddMenuPadding;

                                SetWindowCorners(dropDown.Handle, DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL);
                            }
                        }
                        break;

                    case Button button:
                        button.FlatStyle = FlatStyle.System;
                        break;

                    case Panel:
                    case SettingsPageList:
                        c.BackColor = backColor;
                        c.ForeColor = foreColor;
                        break;

                    case TextBox:
                    case ListBox:
                    case ListView:
                        c.BackColor = effectiveDarkMode ? backColor : default;
                        c.ForeColor = foreColor;
                        break;

                    case RichTextBox richTextBox:
                        c.BackColor = richTextBox.ReadOnly ? backColor : default;
                        c.ForeColor = richTextBox.ReadOnly ? foreColor : default;
                        break;

                    case ComboBox comboBox:
                        comboBox.FlatStyle = effectiveDarkMode ? FlatStyle.Flat : FlatStyle.Standard;
                        comboBox.BackColor = effectiveDarkMode ? backColor : default;
                        comboBox.ForeColor = foreColor;
                        break;

                    case CodeTextBox scintilla:
                        scintilla.Theme = effectiveDarkMode ? Theme.Dark : Theme.Light;
                        break;

                    case LinkLabel linkLabel:
                        linkLabel.ActiveLinkColor = foreColor;
                        linkLabel.LinkColor = foreColor;
                        linkLabel.BackColor = Color.Transparent;
                        break;

                    case Label:
                    case CheckBox:
                        c.BackColor = Color.Transparent;
                        break;

                    case IndicatorBar indicatorBar:
                        indicatorBar.Theme = effectiveDarkMode ? Theme.Dark : Theme.Light;
                        break;
                }

                if (c is IIntelliTipHost intelliTipHost)
                {
                    intelliTipHost.ThemeToolTip(
                        effectiveDarkMode ? Color.White : Color.FromArgb(30, 30, 30),
                        effectiveDarkMode ? Color.FromArgb(40, 40, 40) : Color.WhiteSmoke);
                }

                EnableUxThemeDarkMode(c.Handle, effectiveDarkMode);
            }
        }

        private static void AddMenuPadding(object sender, CancelEventArgs e)
        {
            if (sender is ToolStripDropDownMenu dropDown && dropDown.Items.Count > 0)
            {
                const int menuPadding = 2;

                foreach (ToolStripItem item in dropDown.Items)
                {
                    item.Margin = new Padding(menuPadding, 0, menuPadding, 0);
                }

                dropDown.Items[0].Margin = new Padding(menuPadding, menuPadding, menuPadding, 0);
                dropDown.Items[^1].Margin = new Padding(menuPadding, 0, menuPadding, menuPadding);

                dropDown.AutoSize = true;
                dropDown.AutoSize = false;
                dropDown.Size = new Size(dropDown.Width + (menuPadding * 2), dropDown.Height);
            }
        }

        private static IEnumerable<T> Descendants<T>(this Control control)
            where T : Control
        {
            foreach (Control child in control.Controls)
            {
                if (child is T childOfT)
                {
                    yield return childOfT;
                }

                if (child.HasChildren)
                {
                    foreach (T descendant in Descendants<T>(child))
                    {
                        yield return descendant;
                    }
                }

                if (child.ContextMenuStrip is T contextMenuOfT)
                {
                    yield return contextMenuOfT;
                }
            }
        }

        [DllImport("uxtheme.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        /// <summary>
        /// Use to set dark scroll bars
        /// </summary>
        private static void EnableUxThemeDarkMode(IntPtr hwnd, bool enable)
        {
            string themeName = enable ? "DarkMode_Explorer" : null;
            SetWindowTheme(hwnd, themeName, null);
        }

        internal static void UpdateTheme(this ToolTip toolTip)
        {
            IntPtr handle = toolTip.GetHandle();
            bool effectiveDarkMode = currentTheme == Theme.Dark || (currentTheme == Theme.Auto && isAppThemeDark);
            EnableUxThemeDarkMode(handle, effectiveDarkMode);
            SetWindowCorners(handle, DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL);
        }

        internal static void DrawItemSelection(this Graphics graphics, Color backColor, Rectangle bounds, ItemSelectionFlags itemSelectionFlags = ItemSelectionFlags.Fill | ItemSelectionFlags.Outline)
        {
            if (itemSelectionFlags == 0)
            {
                throw new InvalidEnumArgumentException($"No value provided for {nameof(ItemSelectionFlags)}.");
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Size rectRadius = UIUtil.ScaleSize(6, 6);
            Color selectedColor = ColorBgra.Blend([Color.Gray, backColor]);
            const float pixelOffset = 0.5f; // allows 2px lines to actually be 2px width, and edges of rectangles to be sharp;

            if (itemSelectionFlags.HasFlag(ItemSelectionFlags.Fill))
            {
                RectangleF fillRect = RectangleF.FromLTRB(bounds.Left - pixelOffset, bounds.Top - pixelOffset, bounds.Right - pixelOffset, bounds.Bottom - pixelOffset);
                Color fillColor = Color.FromArgb(128, selectedColor);
                using SolidBrush backBrush = new SolidBrush(fillColor);
                graphics.FillRoundedRectangle(backBrush, fillRect, rectRadius);
            }

            if (itemSelectionFlags.HasFlag(ItemSelectionFlags.AccentOutline) ||
                itemSelectionFlags.HasFlag(ItemSelectionFlags.Outline))
            {
                int outlineWidth = int.Min(UIUtil.Scale(1), 2); // limit width to 2, because GDI sucks
                float outlineOffset = outlineWidth > 1 ? pixelOffset : 0;

                RectangleF outlineRect = RectangleF.FromLTRB(
                    bounds.Left + outlineOffset,
                    bounds.Top + outlineOffset,
                    bounds.Right - outlineOffset - 1,
                    bounds.Bottom - outlineOffset - 1);

                Color outlineColor = itemSelectionFlags.HasFlag(ItemSelectionFlags.AccentOutline)
                    ? AccentColor
                    : selectedColor;

                using Pen outlinePen = new Pen(outlineColor, outlineWidth);
                graphics.DrawRoundedRectangle(outlinePen, outlineRect, rectRadius);
            }

            if (itemSelectionFlags.HasFlag(ItemSelectionFlags.AccentMark))
            {
                Size accentRadius = UIUtil.ScaleSize(3, 3);
                int accentWidth = UIUtil.Scale(3);
                RectangleF accentRect = new RectangleF(bounds.X - pixelOffset, bounds.Y - pixelOffset + (int)float.Round(bounds.Height / 4f), accentWidth, (int)float.Round(bounds.Height / 2f));
                using SolidBrush accentBrush = new SolidBrush(AccentColor);
                graphics.FillRoundedRectangle(accentBrush, accentRect, accentRadius);
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        }

        private static IEnumerable<ToolStrip> Descendants(this ToolStrip toolStrip)
        {
            yield return toolStrip;

            foreach (ToolStripDropDownItem items in toolStrip.Items.OfType<ToolStripDropDownItem>())
            {
                foreach (ToolStrip descendant in items.DropDown.Descendants())
                {
                    yield return descendant;
                }
            }
        }

        private static Color ToGdiColor(this Windows.UI.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [Flags]
        private enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// Use with DwmSetWindowAttribute. Specifies the rounded corner preference for a window. The pvAttribute parameter points to a value of type DWM_WINDOW_CORNER_PREFERENCE. This value is supported starting with Windows 11 Build 22000.
            /// </summary>
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref uint pvAttribute, int cbAttribute);

        private static void SetWindowCorners(IntPtr hwnd, DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
            {
                return;
            }

            uint cornerPreferenceUInt = (uint)cornerPreference;
            DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreferenceUInt, sizeof(DWM_WINDOW_CORNER_PREFERENCE));
        }

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_ToolTip")]
        private static extern ToolTip GetToolTip(this ToolStrip toolStrip);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Handle")]
        private static extern IntPtr GetHandle(this ToolTip toolStrip);

        internal static void InvalidateCache()
        {
            accentColor = Color.Empty;
        }

        private sealed class ThemeRenderer : ToolStripProfessionalRenderer
        {
            internal ThemeRenderer() : base(new ThemeColorTable())
            {
                RoundedEdges = false;
            }

            private Dictionary<ToolStripItem, ToolStripArrowRenderEventArgs> itemArrowArgs = new Dictionary<ToolStripItem, ToolStripArrowRenderEventArgs>();

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = ThemeUtil.foreColor;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                if (e.Item.Enabled)
                {
                    e.ArrowColor = ThemeUtil.foreColor;
                }

                itemArrowArgs[e.Item] = e;

                base.OnRenderArrow(e);
            }

            protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderItemBackground(e);

                e.Graphics.FillRoundedRectangle(Brushes.Red, e.Item.ContentRectangle, new Size(6, 6));
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderMenuItemBackground(e);

                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle itemRect = e.ToolStrip is MenuStrip
                        ? new Rectangle(Point.Empty, e.Item.Bounds.Size)
                        : e.Item.ContentRectangle;

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect);
                }
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderButtonBackground(e);

                bool isChecked = e.Item is ToolStripButton button && button.Checked;

                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle itemRect = new Rectangle(Point.Empty, e.Item.Bounds.Size);

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    ItemSelectionFlags selectionFlags = isChecked
                        ? ItemSelectionFlags.Fill | ItemSelectionFlags.AccentOutline
                        : ItemSelectionFlags.Fill | ItemSelectionFlags.Outline;

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect, selectionFlags);
                }
                else if (isChecked)
                {
                    Rectangle itemRect = new Rectangle(Point.Empty, e.Item.Bounds.Size);

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect, ItemSelectionFlags.AccentOutline);
                }
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                //base.OnRenderItemCheck(e);

                e.Graphics.DrawImage(e.Image, e.ImageRectangle);
            }

            protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderDropDownButtonBackground(e);

                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle itemRect = new Rectangle(e.Item.ContentRectangle.Location, e.Item.Bounds.Size);

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect);

                    if (itemArrowArgs.TryGetValue(e.Item, out ToolStripArrowRenderEventArgs arrowArgs))
                    {
                        ToolStripArrowRenderEventArgs newArrowArgs = new ToolStripArrowRenderEventArgs(
                            e.Graphics, arrowArgs.Item, arrowArgs.ArrowRectangle, arrowArgs.ArrowColor, arrowArgs.Direction);

                        OnRenderArrow(newArrowArgs);
                    }
                }
            }

            protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderSplitButtonBackground(e);

                if (e.Item.Selected || e.Item.Pressed)
                {
                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, e.Item.ContentRectangle);

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, e.Item.ContentRectangle);

                    if (itemArrowArgs.TryGetValue(e.Item, out ToolStripArrowRenderEventArgs arrowArgs))
                    {
                        if (!e.Item.Pressed)
                        {
                            using Pen linePen = new Pen(ColorTable.MenuBorder, UIUtil.Scale(1));
                            e.Graphics.DrawLine(
                                linePen,
                                arrowArgs.ArrowRectangle.Left - 1,
                                arrowArgs.ArrowRectangle.Top + UIUtil.Scale(1),
                                arrowArgs.ArrowRectangle.Left - 1,
                                arrowArgs.ArrowRectangle.Bottom - UIUtil.Scale(1));
                        }

                        OnRenderArrow(arrowArgs);
                    }
                }
            }

            private sealed class ThemeColorTable : ProfessionalColorTable
            {
                private readonly Color BackColor = ThemeUtil.backColor;
                private readonly Color BorderColor = ThemeUtil.backColor; //Color.FromArgb(186, 0, 105, 210);
                private readonly Color HiliteColor = ThemeUtil.backColor; //Color.FromArgb(62, 0, 103, 206);
                private readonly Color CheckedColor = ThemeUtil.AccentColor; //Color.FromArgb(129, 52, 153, 254);
                private readonly Color CheckedBorderColor = ThemeUtil.backColor; //Color.FromArgb(52, 153, 254);

                public override Color ButtonSelectedHighlight => HiliteColor;
                public override Color ButtonSelectedBorder => BorderColor;
                public override Color ButtonSelectedGradientBegin => HiliteColor;
                public override Color ButtonSelectedGradientMiddle => HiliteColor;
                public override Color ButtonSelectedGradientEnd => HiliteColor;
                public override Color ButtonSelectedHighlightBorder => BorderColor;

                public override Color ButtonPressedHighlight => HiliteColor;
                public override Color ButtonPressedGradientBegin => CheckedColor;
                public override Color ButtonPressedGradientMiddle => CheckedColor;
                public override Color ButtonPressedGradientEnd => CheckedColor;
                public override Color ButtonPressedBorder => CheckedBorderColor;
                public override Color ButtonPressedHighlightBorder => CheckedBorderColor;

                public override Color ButtonCheckedGradientBegin => CheckedColor;
                public override Color ButtonCheckedGradientMiddle => CheckedColor;
                public override Color ButtonCheckedGradientEnd => CheckedColor;
                public override Color ButtonCheckedHighlight => CheckedColor;
                public override Color ButtonCheckedHighlightBorder => CheckedBorderColor;

                public override Color ToolStripBorder => Color.Transparent;
                public override Color ToolStripGradientBegin => BackColor;
                public override Color ToolStripGradientMiddle => BackColor;
                public override Color ToolStripGradientEnd => BackColor;
                public override Color ToolStripDropDownBackground => BackColor;

                public override Color MenuItemBorder => BorderColor;
                public override Color MenuItemPressedGradientBegin => BackColor;
                public override Color MenuItemPressedGradientMiddle => BackColor;
                public override Color MenuItemPressedGradientEnd => BackColor;

                public override Color MenuItemSelected => HiliteColor;
                public override Color MenuItemSelectedGradientBegin => HiliteColor;
                public override Color MenuItemSelectedGradientEnd => HiliteColor;

                public override Color CheckBackground => CheckedColor;
                public override Color CheckSelectedBackground => HiliteColor;
                public override Color CheckPressedBackground => CheckedColor;

                public override Color MenuStripGradientBegin => BackColor;
                public override Color MenuStripGradientEnd => BackColor;
                public override Color MenuBorder => ColorBgra.Blend([Color.Gray, BackColor]);

                public override Color ImageMarginGradientBegin => BackColor;
                public override Color ImageMarginGradientMiddle => BackColor;
                public override Color ImageMarginGradientEnd => BackColor;

                public override Color SeparatorLight => BackColor;
                public override Color SeparatorDark => Color.FromArgb(85, 85, 85);
            }
        }

        private sealed class TabRenderer : ToolStripProfessionalRenderer
        {
            internal TabRenderer() : base(new TabColorTable())
            {
                RoundedEdges = false;
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = ThemeUtil.foreColor;
                e.TextRectangle = new Rectangle(e.TextRectangle.X + UIUtil.Scale(e.Item.Padding.Left), e.TextRectangle.Y, e.TextRectangle.Width, e.TextRectangle.Height);
                base.OnRenderItemText(e);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderButtonBackground(e);

                bool isChecked = e.Item is ToolStripButton button && button.Checked;

                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle itemRect = new Rectangle(Point.Empty, e.Item.Bounds.Size);

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    ItemSelectionFlags selectionFlags = isChecked
                        ? ItemSelectionFlags.Fill | ItemSelectionFlags.AccentOutline
                        : ItemSelectionFlags.Fill | ItemSelectionFlags.Outline;

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect, selectionFlags);
                }
                else if (isChecked)
                {
                    Rectangle itemRect = new Rectangle(Point.Empty, e.Item.Bounds.Size);

                    using SolidBrush clearBrush = new SolidBrush(ColorTable.ToolStripDropDownBackground);
                    e.Graphics.FillRectangle(clearBrush, itemRect);

                    e.Graphics.DrawItemSelection(ColorTable.ToolStripDropDownBackground, itemRect, ItemSelectionFlags.AccentOutline);
                }
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {
                Rectangle newImageRect = new Rectangle(e.ImageRectangle.X + UIUtil.Scale(e.Item.Padding.Left), e.ImageRectangle.Y, e.ImageRectangle.Width, e.ImageRectangle.Height);
                ToolStripItemImageRenderEventArgs newEventArgs = new ToolStripItemImageRenderEventArgs(e.Graphics, e.Item, e.Image, newImageRect);

                base.OnRenderItemImage(newEventArgs);
            }

            private sealed class TabColorTable : ProfessionalColorTable
            {
                internal TabColorTable()
                {
                    UseSystemColors = false;
                }

                private readonly Color BackColor = Color.FromArgb(255, ColorBgra.Blend(new ColorBgra[] { ColorBgra.FromBgra(128, 128, 128, 64), ThemeUtil.backColor }));
                private readonly Color BorderColor = ThemeUtil.backColor; // Color.FromArgb(186, 0, 105, 210);
                private readonly Color HiliteColor = Color.FromArgb(62, 0, 103, 206);
                private readonly Color CheckedColor = Color.FromArgb(255, ColorBgra.Blend(new ColorBgra[] { ColorBgra.FromBgra(128, 128, 128, 64), ThemeUtil.backColor })); //Color.FromArgb(129, 52, 153, 254);
                private readonly Color ActiveTabColor = ThemeUtil.AccentColor; // Color.FromArgb(255, 52, 153, 254);

                public override Color ButtonSelectedHighlight => HiliteColor;
                public override Color ButtonSelectedBorder => ActiveTabColor;
                public override Color ButtonSelectedGradientBegin => HiliteColor;
                public override Color ButtonSelectedGradientMiddle => HiliteColor;
                public override Color ButtonSelectedGradientEnd => HiliteColor;
                public override Color ButtonSelectedHighlightBorder => BorderColor;

                public override Color ButtonPressedHighlight => HiliteColor;
                public override Color ButtonPressedGradientBegin => CheckedColor;
                public override Color ButtonPressedGradientMiddle => CheckedColor;
                public override Color ButtonPressedGradientEnd => CheckedColor;
                public override Color ButtonPressedBorder => BorderColor;
                public override Color ButtonPressedHighlightBorder => BorderColor;

                public override Color ButtonCheckedGradientBegin => CheckedColor;
                public override Color ButtonCheckedGradientMiddle => CheckedColor;
                public override Color ButtonCheckedGradientEnd => CheckedColor;
                public override Color ButtonCheckedHighlight => CheckedColor;
                public override Color ButtonCheckedHighlightBorder => ActiveTabColor;

                public override Color ToolStripBorder => Color.Transparent;
                public override Color ToolStripGradientBegin => BackColor;
                public override Color ToolStripGradientMiddle => BackColor;
                public override Color ToolStripGradientEnd => BackColor;
                public override Color ToolStripDropDownBackground => BackColor;

                public override Color OverflowButtonGradientBegin => BackColor;
                public override Color OverflowButtonGradientMiddle => BackColor;
                public override Color OverflowButtonGradientEnd => BackColor;
            }
        }
    }
}
