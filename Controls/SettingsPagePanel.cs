using System.Windows.Forms;

namespace PdnCodeLab
{
    public class SettingsPagePanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            e.Graphics.DrawItemSelection(this.BackColor, this.ClientRectangle, ItemSelectionFlags.Fill | ItemSelectionFlags.Outline);
        }
    }
}
