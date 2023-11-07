using PaintDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PdnCodeLab
{
    public class ChildFormBase : PdnBaseForm
    {
        private string iconName = string.Empty;

        public ChildFormBase()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(800, 450);
            this.Font = SystemFonts.MenuFont;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = string.Empty;
            this.ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.UpdateTheme();

            base.OnLoad(e);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Icon Icon
        {
            get => base.Icon;
            set => base.Icon = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        public string IconName
        {
            get => this.iconName;
            set
            {
                this.iconName = value.Trim();
                this.Icon = UIUtil.CreateIcon(this.iconName);
            }
        }
    }
}
