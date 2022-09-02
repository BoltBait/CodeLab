using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class IntelliTip : ToolTip
    {
        internal bool Visible => visibile;
        private bool visibile = false;
        private Color? color;

        internal IntelliTip()
        {
            this.UseFading = false;
            this.OwnerDraw = true;
            this.Draw += IntelliTip_Draw;
            this.Popup += IntelliTip_Popup;
        }

        private void IntelliTip_Popup(object sender, PopupEventArgs e)
        {
            int extraWidth = this.color.HasValue ? 28 : 10;
            e.ToolTipSize = TextRenderer.MeasureText(this.GetToolTip(e.AssociatedControl), SystemFonts.MessageBoxFont, Size.Empty, TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding) + new Size(extraWidth, 14);
        }

        private void IntelliTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            Rectangle bounds = Rectangle.FromLTRB(e.Bounds.Left + 6, e.Bounds.Top, e.Bounds.Right - 6, e.Bounds.Bottom);
            TextRenderer.DrawText(e.Graphics, e.ToolTipText, SystemFonts.MessageBoxFont, bounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding);

            if (this.color.HasValue)
            {
                const int padding = 9;
                const int swatchHeight = 12;

                Rectangle rect = new Rectangle(
                    e.Bounds.Right - swatchHeight - padding,
                    e.Bounds.Top + padding,
                    swatchHeight,
                    swatchHeight);

                Rectangle innerRect = Rectangle.FromLTRB(
                    rect.Left + 1,
                    rect.Top + 1,
                    rect.Right,
                    rect.Bottom);

                using SolidBrush brush = new SolidBrush(this.color.Value);
                e.Graphics.FillRectangle(brush, innerRect);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }
        }

        internal void UpdateTheme(Color foreColor, Color backColor)
        {
            this.ForeColor = foreColor;
            this.BackColor = backColor;
        }

        internal new void Show(string text, IWin32Window window, int x, int y)
        {
            Match colorMatch = Regex.Match(text, @"^(Color|ColorBgra)\b - (Color|ColorBgra)\b.(?<NamedColor>\w+) { get; }\W+Property$");
            this.color = colorMatch.Success && Enum.TryParse(colorMatch.Groups["NamedColor"].Value, false, out KnownColor knownColor)
                ? Color.FromKnownColor(knownColor)
                : null;

            base.Show(text, window, x, y);
            visibile = true;
        }

        internal new void Hide(IWin32Window win)
        {
            base.Hide(win);
            visibile = false;
        }
    }
}
