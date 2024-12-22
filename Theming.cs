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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PdnCodeLab
{
    public enum Theme
    {
        Auto,
        Light,
        Dark
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
                            ts.TryGetToolTip()?.UpdateTheme();
                        }

                        break;

                    case Button button:
                        button.FlatStyle = FlatStyle.System;
                        break;

                    case TextBox:
                    case ListBox:
                    case ListView:
                        c.BackColor = effectiveDarkMode ? backColor : default;
                        c.ForeColor = foreColor;
                        break;

                    case Panel:
                        c.BackColor = backColor;
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
                        break;

                    case IndicatorBar indicatorBar:
                        indicatorBar.Theme = effectiveDarkMode ? Theme.Dark : Theme.Light;
                        break;
                }

                if (c is IIntelliTipHost intelliTipHost)
                {
                    intelliTipHost.ThemeToolTip(
                        effectiveDarkMode ? Color.White : Color.FromArgb(30, 30, 30),
                        effectiveDarkMode ? Color.FromArgb(66, 66, 66) : Color.WhiteSmoke);
                }

                EnableUxThemeDarkMode(c.Handle, effectiveDarkMode);
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
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            {
                return;
            }

            string themeName = enable ? "DarkMode_Explorer" : null;
            SetWindowTheme(hwnd, themeName, null);
        }

        internal static void UpdateTheme(this ToolTip toolTip)
        {
            IntPtr handle = toolTip.TryGetHandle();
            if (handle != IntPtr.Zero)
            {
                bool effectiveDarkMode = currentTheme == Theme.Dark || (currentTheme == Theme.Auto && isAppThemeDark);
                EnableUxThemeDarkMode(handle, effectiveDarkMode);
                SetWindowCorners(handle, true);
            }
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

        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute);

        private static void SetWindowCorners(IntPtr hwnd, bool rounded)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
            {
                return;
            }

            DWM_WINDOW_CORNER_PREFERENCE cornerPreference = rounded ? DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL : DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;
            DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreference, sizeof(DWM_WINDOW_CORNER_PREFERENCE));
        }

        private static ToolTip TryGetToolTip(this ToolStrip toolStrip)
        {
            object ToolTipObj = typeof(ToolStrip)
                .GetProperty("ToolTip", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(toolStrip);

            return ToolTipObj is ToolTip toolTip
                ? toolTip
                : null;
        }

        private static IntPtr TryGetHandle(this ToolTip toolTip)
        {
            object handleObj = typeof(ToolTip)
                .GetProperty("Handle", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(toolTip);

            return handleObj is IntPtr handleIntPtr
                ? handleIntPtr
                : IntPtr.Zero;
        }

        private sealed class ThemeRenderer : ToolStripProfessionalRenderer
        {
            internal ThemeRenderer() : base(new ThemeColorTable())
            {
                RoundedEdges = false;
            }

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

                base.OnRenderArrow(e);
            }

            private sealed class ThemeColorTable : ProfessionalColorTable
            {
                internal ThemeColorTable()
                {
                    UseSystemColors = false;
                }

                private readonly Color BackColor = ThemeUtil.backColor;
                private readonly Color BorderColor = Color.FromArgb(186, 0, 105, 210);
                private readonly Color HiliteColor = Color.FromArgb(62, 0, 103, 206);
                private readonly Color CheckedColor = Color.FromArgb(129, 52, 153, 254);
                private readonly Color CheckedBorderColor = Color.FromArgb(52, 153, 254);

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

                public override Color ToolStripBorder => BackColor;
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
                public override Color MenuBorder => Color.Gray;

                public override Color ImageMarginGradientBegin => BackColor;
                public override Color ImageMarginGradientMiddle => BackColor;
                public override Color ImageMarginGradientEnd => BackColor;

                public override Color SeparatorLight => BackColor;
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
                base.OnRenderItemText(e);
            }

            private sealed class TabColorTable : ProfessionalColorTable
            {
                internal TabColorTable()
                {
                    UseSystemColors = false;
                }

                private readonly Color BackColor = Color.FromArgb(255, ColorBgra.Blend(new ColorBgra[] { ColorBgra.FromBgra(128, 128, 128, 64), ThemeUtil.backColor }));
                private readonly Color BorderColor = Color.FromArgb(186, 0, 105, 210);
                private readonly Color HiliteColor = Color.FromArgb(62, 0, 103, 206);
                private readonly Color CheckedColor = Color.FromArgb(129, 52, 153, 254);
                private readonly Color ActiveTabColor = Color.FromArgb(255, 52, 153, 254);

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

                public override Color ToolStripBorder => ActiveTabColor;
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
