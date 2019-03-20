using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public sealed class ScaledToolStripButton : ToolStripButton
    {
        public ScaledToolStripButton()
        {
            this.DisplayStyle = ToolStripItemDisplayStyle.Image;
        }

        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
            }
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
        public ScaledToolStripMenuItem()
        {
        }

        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
            }
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
