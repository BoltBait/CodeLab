using PaintDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PdnCodeLab
{

    public class SettingsPageList : ListBox
    {
        public SettingsPageList()
        {
            this.Items.AddRange(SettingsPageListItem.Items);
            this.BorderStyle = BorderStyle.None;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = UIUtil.Scale(32);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal SettingsPage SelectedPage
        {
            get => (SettingsPage)this.SelectedIndex;
            set => this.SelectedIndex = (int)value;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1 || this.Items[e.Index] is not SettingsPageListItem item)
            {
                return;
            }

            using SolidBrush clearBrush = new SolidBrush(this.BackColor);
            e.Graphics.FillRectangle(clearBrush, e.Bounds);

            const int itemMargin = 0;
            Rectangle itemRect = Rectangle.FromLTRB(e.Bounds.Left + itemMargin, e.Bounds.Top, e.Bounds.Right - itemMargin - 1, e.Bounds.Bottom - 1);

            if (e.State.HasFlag(DrawItemState.Selected))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Color selectedColor = ColorBgra.Blend([Color.Gray, this.BackColor]);
                Size rectRadius = new Size(6, 6);

                Color fillColor = Color.FromArgb(128, selectedColor);
                using SolidBrush backBrush = new SolidBrush(fillColor);
                e.Graphics.FillRoundedRectangle(backBrush, itemRect, rectRadius);

                using Pen outlinePen = new Pen(selectedColor);
                e.Graphics.DrawRoundedRectangle(outlinePen, itemRect, rectRadius);

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }

            Rectangle iconRect = new Rectangle(e.Bounds.X + UIUtil.Scale(4), e.Bounds.Y + UIUtil.Scale(4), UIUtil.Scale(24), UIUtil.Scale(24));
            e.Graphics.DrawImage(item.Image, iconRect);

            Rectangle textBounds = new Rectangle(e.Bounds.X + e.Bounds.Height, e.Bounds.Y, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, item.Text, e.Font, textBounds, this.ForeColor, TextFormatFlags.VerticalCenter);

            base.OnDrawItem(e);
        }

        private class SettingsPageListItem
        {
            internal Image Image { get; }
            internal string Text { get; }

            private SettingsPageListItem(SettingsPage settingsPage)
            {
                Image = UIUtil.GetImage(settingsPage.ToString());
                Text = settingsPage switch
                {
                    SettingsPage.UI => "User Interface",
                    SettingsPage.Snippet => "Snippets",
                    SettingsPage.Spelling => "Spellcheck",
                    SettingsPage.Compiler => "Compiler",
                    SettingsPage.Assistance => "Assistance",
                    SettingsPage.RenderOptions => "Render Options",
                    SettingsPage.Updates => "Updates",
                    _ => throw new NotImplementedException(),
                };
            }

            public override string ToString()
            {
                return Text;
            }

            internal static SettingsPageListItem[] Items { get; } = Enum.GetValues<SettingsPage>().Select(x => new SettingsPageListItem(x)).ToArray();
        }

    }

    internal enum SettingsPage
    {
        UI,
        Assistance,
        Snippet,
        Spelling,
        Compiler,
        RenderOptions,
        Updates
    }
}
