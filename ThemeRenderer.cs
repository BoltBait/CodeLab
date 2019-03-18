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

using System.Drawing;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class ThemeRenderer : ToolStripProfessionalRenderer
    {
        internal ThemeRenderer() : base(new ThemeColorTable())
        {
            RoundedEdges = false;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = PdnTheme.ForeColor;
            base.OnRenderItemText(e);
        }

        private sealed class ThemeColorTable : ProfessionalColorTable
        {
            internal ThemeColorTable()
            {
                UseSystemColors = false;
            }

            private readonly Color BackColor = PdnTheme.BackColor;
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

    internal sealed class TabRenderer : ToolStripProfessionalRenderer
    {
        internal TabRenderer() : base(new TabColorTable())
        {
            RoundedEdges = false;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = PdnTheme.ForeColor;
            base.OnRenderItemText(e);
        }

        private sealed class TabColorTable : ProfessionalColorTable
        {
            internal TabColorTable()
            {
                UseSystemColors = false;
            }

            private readonly Color BackColor = Color.FromArgb(255, ColorBgra.Blend(new ColorBgra[] { ColorBgra.FromBgra(128, 128, 128, 64), PdnTheme.BackColor }));
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

    internal static class PdnTheme
    {
        private static Color foreColor;
        private static Color backColor;
        private static ThemeRenderer themeRenderer;
        private static TabRenderer tabRenderer;

        internal static void SetColors(Color foreColor, Color backColor)
        {
            PdnTheme.foreColor = foreColor;
            PdnTheme.backColor = backColor;
            PdnTheme.themeRenderer = null;
            PdnTheme.tabRenderer = null;
        }

        internal static Theme Theme => (PdnTheme.backColor.R < 128 && PdnTheme.backColor.G < 128 && PdnTheme.backColor.B < 128) ? Theme.Dark : Theme.Light;

        internal static Color ForeColor => foreColor;
        internal static Color BackColor => backColor;

        internal static ThemeRenderer Renderer
        {
            get
            {
                if (themeRenderer == null)
                {
                    themeRenderer = new ThemeRenderer();
                }

                return themeRenderer;
            }
        }

        internal static TabRenderer TabRenderer
        {
            get
            {
                if (tabRenderer == null)
                {
                    tabRenderer = new TabRenderer();
                }

                return tabRenderer;
            }
        }
    }
}
