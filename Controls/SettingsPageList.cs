using PaintDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PdnCodeLab
{

    public class SettingsPageList : ListBox
    {
        private int indexAtMouse = -1;

        public SettingsPageList()
        {
            this.Items.AddRange(SettingsPageListItem.Items);
            this.BorderStyle = BorderStyle.None;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = UIUtil.Scale(36);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal SettingsPage SelectedPage
        {
            get => (SettingsPage)this.SelectedIndex;
            set => this.SelectedIndex = (int)value;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int newIndexAtMouse = this.IndexFromPoint(e.Location);

            if (newIndexAtMouse != this.indexAtMouse)
            {
                int oldIndexAtMouse = this.indexAtMouse;
                this.indexAtMouse = newIndexAtMouse;

                if (oldIndexAtMouse > -1)
                {
                    Rectangle oldIndexRect = this.GetItemRectangle(oldIndexAtMouse);
                    this.Invalidate(oldIndexRect);
                }

                if (newIndexAtMouse > -1)
                {
                    Rectangle indexRect = this.GetItemRectangle(newIndexAtMouse);
                    this.Invalidate(indexRect);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            int oldIndexAtMouse = this.indexAtMouse;
            this.indexAtMouse = -1;

            if (oldIndexAtMouse > -1)
            {
                Rectangle oldIndexRect = this.GetItemRectangle(oldIndexAtMouse);
                this.Invalidate(oldIndexRect);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1 || this.Items[e.Index] is not SettingsPageListItem item)
            {
                return;
            }

            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;

            Rectangle itemBounds = new Rectangle(Point.Empty, e.Bounds.Size);
            using BufferedGraphics bg = currentContext.Allocate(e.Graphics, itemBounds);

            using SolidBrush clearBrush = new SolidBrush(this.BackColor);
            bg.Graphics.FillRectangle(clearBrush, itemBounds);

            if (e.State.HasFlag(DrawItemState.Selected) || e.Index == this.indexAtMouse)
            {
                ItemSelectionFlags itemSelectionFlags = e.State.HasFlag(DrawItemState.Selected)
                    ? ItemSelectionFlags.Fill | ItemSelectionFlags.AccentMark
                    : ItemSelectionFlags.Fill;

                Rectangle itemSelectionRect = Rectangle.FromLTRB(itemBounds.Left, itemBounds.Top + 2, itemBounds.Right, itemBounds.Bottom - 2);
                bg.Graphics.DrawItemSelection(this.BackColor, itemSelectionRect, itemSelectionFlags);
            }

            int accentPadding = UIUtil.Scale(4);

            Rectangle iconRect = new Rectangle(itemBounds.X + accentPadding + UIUtil.Scale(4), itemBounds.Y + UIUtil.Scale(6), UIUtil.Scale(24), UIUtil.Scale(24));
            bg.Graphics.DrawImage(item.Image, iconRect);

            Rectangle textBounds = new Rectangle(itemBounds.X + accentPadding + itemBounds.Height, itemBounds.Y, itemBounds.Width - itemBounds.Height, itemBounds.Height);
            TextRenderer.DrawText(bg.Graphics, item.Text, e.Font, textBounds, this.ForeColor, TextFormatFlags.VerticalCenter);

            // Wrapper around BitBlt
            CopyGraphics(e.Graphics, e.Bounds, bg.Graphics, itemBounds.Location);

            base.OnDrawItem(e);
        }

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        private static void CopyGraphics(Graphics dstGraphics, Rectangle dstBounds, Graphics srcGraphics, Point srcLocation)
        {
            IntPtr dstHdc = dstGraphics.GetHdc();
            IntPtr srcHdc = srcGraphics.GetHdc();

            const uint SRCCOPY = 0x00CC0020;
            BitBlt(dstHdc, dstBounds.X, dstBounds.Y, dstBounds.Width, dstBounds.Height, srcHdc, srcLocation.X, srcLocation.Y, SRCCOPY);

            dstGraphics.ReleaseHdc(dstHdc);
            srcGraphics.ReleaseHdc(srcHdc);
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
