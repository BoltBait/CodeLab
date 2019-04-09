using System.Drawing;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    internal sealed class IntelliTip : ToolTip
    {
        internal bool Visible => visibile;
        private bool visibile = false;

        internal IntelliTip()
        {
            this.UseFading = false;
            this.OwnerDraw = true;
            this.Draw += IntelliTip_Draw;
            this.Popup += IntelliTip_Popup;
        }

        private void IntelliTip_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText(this.GetToolTip(e.AssociatedControl), SystemFonts.MessageBoxFont, Size.Empty, TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding) + new Size(11, 14);
        }

        private void IntelliTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            Rectangle bounds = Rectangle.FromLTRB(e.Bounds.Left + 8, e.Bounds.Top + 7, e.Bounds.Right - 8, e.Bounds.Bottom - 7);
            TextRenderer.DrawText(e.Graphics, e.ToolTipText, SystemFonts.MessageBoxFont, bounds, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        internal void UpdateTheme(Color foreColor, Color backColor)
        {
            this.ForeColor = foreColor;
            this.BackColor = backColor;
        }

        internal new void Show(string text, IWin32Window window, int x, int y)
        {
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
