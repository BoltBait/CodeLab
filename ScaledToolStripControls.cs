using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PaintDotNet.Effects
{
    public class ScaledToolStripButton : ToolStripButton
    {
        public ScaledToolStripButton()
        {
            this.Padding = new Padding(2);
        }

        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value.Trim();
                this.Image = UIUtil.GetImage(imageName);
            }
        }
    }

    public class ScaledToolStripSplitButton : ToolStripSplitButton
    {
        public ScaledToolStripSplitButton()
        {
            this.Padding = new Padding(2);
        }

        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value.Trim();
                this.Image = UIUtil.GetImage(imageName);
            }
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

        [Category(nameof(CategoryAttribute.Appearance))]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value.Trim();
                this.Image = UIUtil.GetImage(imageName);
            }
        }
    }

    public sealed class ScaledToolStripDropDownButton : ToolStripDropDownButton
    {
        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value.Trim();
                this.Image = UIUtil.GetImage(imageName);
            }
        }
    }

    public sealed class ScaledButton : Button
    {
        private string imageName = string.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image
        {
            get => base.Image;
            set => base.Image = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        public string ImageName
        {
            get => imageName;
            set
            {
                imageName = value.Trim();
                this.Image = UIUtil.GetImage(imageName);
            }
        }
    }
}
