using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public class ScaledToolStripButton : ToolStripButton
    {
        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category("Appearance")]
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value.Trim();
                this.Image = ResUtil.GetImage(imageName);
            }
        }

        // Execute on mid-session DPI change
        internal void SetImage()
        {
            this.Image = ResUtil.GetImage(imageName);
        }
    }

    public sealed class ScaledToolStripMenuItem : ToolStripMenuItem
    {
        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category("Appearance")]
        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value.Trim();
                this.Image = ResUtil.GetImage(imageName);
            }
        }

        // Execute on mid-session DPI change
        internal void SetImage()
        {
            this.Image = ResUtil.GetImage(imageName);
        }
    }
}
