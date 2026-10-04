using PaintDotNet;
using PaintDotNet.Controls;
using PaintDotNet.Direct2D1;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PdnCodeLab
{
    internal sealed class IndicatorBar : Direct2DControl
    {
        private RectInt32 upButtonRect;
        private RectInt32 downButtonRect;
        private RectInt32 thumbRect;
        private RectInt32 shaftRect;

        private bool upButtonHovered;
        private bool downButtonHovered;
        private bool thumbHovered;
        private bool shaftHovered;

        private bool upButtonClicked;
        private bool downButtonClicked;
        private bool thumbClicked;
        private bool shaftClicked;

        private ColorRgb24 shaftColor;
        private ColorRgb24 normalColor;
        private ColorRgb24 hoveredColor;
        private ColorRgb24 clickedColor;

        private ColorRgb24 caretColor;
        private ColorRgb24 errorColor;
        private ColorRgb24 warningColor;
        private ColorRgb24 matchColor;
        private ColorRgb24 bookmarkColor;

        private int clickedShaftPos;
        private int clickedThumbPos;
        private readonly Timer arrowTimer = new Timer();
        private int scrollDirection;


        private Theme theme = Theme.Light;
        private int caret = 0;
        private IEnumerable<int> errors = Array.Empty<int>();
        private IEnumerable<int> warnings = Array.Empty<int>();
        private IEnumerable<int> matches = Array.Empty<int>();
        private IEnumerable<int> bookmarks = Array.Empty<int>();
        private int maximum = 100;
        private int largeChange = 50;


        internal event EventHandler<ScrollEventArgs> Scroll;
        private void OnScroll(ScrollEventArgs args)
        {
            this.Scroll?.Invoke(this, args);
        }

        #region Properties
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal Theme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
                switch (value)
                {
                    case Theme.Dark:
                        shaftColor = new ColorRgb24(62, 62, 66);
                        normalColor = new ColorRgb24(104, 104, 104);
                        hoveredColor = new ColorRgb24(158, 158, 158);
                        clickedColor = new ColorRgb24(239, 235, 239);

                        caretColor = SrgbColors.Gainsboro;
                        errorColor = new ColorRgb24(252, 62, 54);
                        warningColor = new ColorRgb24(149, 219, 125);
                        matchColor = SrgbColors.Orange;
                        bookmarkColor = SrgbColors.DeepSkyBlue;
                        break;

                    case Theme.Light:
                    default:
                        shaftColor = new ColorRgb24(245, 245, 245);
                        normalColor = new ColorRgb24(194, 195, 201);
                        hoveredColor = new ColorRgb24(104, 104, 104);
                        clickedColor = new ColorRgb24(91, 91, 91);

                        caretColor = new ColorRgb24(0, 0, 205);
                        errorColor = SrgbColors.Red;
                        warningColor = SrgbColors.Green;
                        matchColor = new ColorRgb24(246, 185, 77);
                        bookmarkColor = SrgbColors.DeepSkyBlue;
                        break;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int Caret
        {
            get
            {
                return caret;
            }
            set
            {
                caret = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal IEnumerable<int> Errors
        {
            get
            {
                return errors;
            }
            set
            {
                errors = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal IEnumerable<int> Warnings
        {
            get
            {
                return warnings;
            }
            set
            {
                warnings = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal IEnumerable<int> Matches
        {
            get
            {
                return matches;
            }
            set
            {
                matches = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal IEnumerable<int> Bookmarks
        {
            get
            {
                return bookmarks;
            }
            set
            {
                bookmarks = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int Value
        {
            get
            {
                float scale = shaftRect.Height / (float)maximum;
                return (int)float.Round((thumbRect.Y - shaftRect.Top) / scale);
            }
            set
            {
                float scale = shaftRect.Height / (float)maximum;
                thumbRect.Y = shaftRect.Top + (int)float.Round(value * scale);
                if (thumbRect.Top < shaftRect.Top)
                {
                    thumbRect.Y = shaftRect.Top;
                }
                else if (thumbRect.Bottom > shaftRect.Bottom)
                {
                    thumbRect.Y = shaftRect.Bottom - thumbRect.Height;
                }

                Refresh(); // Need to redraw very quickly here. Refresh() rather than Invalidate().
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;

                if (largeChange > maximum)
                {
                    thumbRect.Height = shaftRect.Height;
                }
                else
                {
                    thumbRect.Height = largeChange * shaftRect.Height / value;
                }

                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal int LargeChange
        {
            get
            {
                return largeChange;
            }
            set
            {
                largeChange = value;

                if (largeChange > maximum)
                {
                    thumbRect.Height = shaftRect.Height;
                }
                else
                {
                    thumbRect.Height = value * shaftRect.Height / maximum;
                }

                Invalidate();
            }
        }
        #endregion

        internal IndicatorBar()
        {
            int width = SystemInformation.VerticalScrollBarWidth;
            base.Width = width;

            this.upButtonRect.Size = new SizeInt32(width, width);
            this.downButtonRect.Size = new SizeInt32(width, width);

            this.thumbRect.Width = width;

            this.shaftRect = RectInt32.FromEdges(this.ClientRectangle.Left, upButtonRect.Bottom + 1, this.ClientRectangle.Right, upButtonRect.Bottom + 10);

            this.arrowTimer.Enabled = false;
            this.arrowTimer.Interval = 500;
            this.arrowTimer.Tick += (sender, e) => scrollByDelta();

            this.Theme = Theme.Light;

            base.Dock = DockStyle.Right;
            base.Cursor = Cursors.Default;
            base.DoubleBuffered = true;
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (base.Width != SystemInformation.VerticalScrollBarWidth)
            {
                base.Width = SystemInformation.VerticalScrollBarWidth;
            }

            downButtonRect.Y = this.ClientRectangle.Bottom - downButtonRect.Height;

            shaftRect.Height = downButtonRect.Top - upButtonRect.Bottom - 2;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (upButtonRect.Contains(e.Location))
            {
                if (!upButtonHovered)
                {
                    upButtonHovered = true;
                    this.Invalidate();
                }
            }
            else if (upButtonHovered)
            {
                upButtonHovered = false;
                this.Invalidate();
            }

            if (downButtonRect.Contains(e.Location))
            {
                if (!downButtonHovered)
                {
                    downButtonHovered = true;
                    this.Invalidate();
                }
            }
            else if (downButtonHovered)
            {
                downButtonHovered = false;
                this.Invalidate();
            }

            if (thumbRect.Contains(e.Location))
            {
                if (!thumbHovered)
                {
                    thumbHovered = true;
                    this.Invalidate();
                }
            }
            else if (thumbHovered)
            {
                thumbHovered = false;
                this.Invalidate();
            }

            if (!thumbRect.Contains(e.Location) && shaftRect.Contains(e.Location))
            {
                if (scrollDirection == -1 && e.Y > thumbRect.Top)
                {
                    scrollDirection = 0;
                }
                else if (scrollDirection == 1 && e.Y < thumbRect.Bottom)
                {
                    scrollDirection = 0;
                }

                if (!shaftHovered)
                {
                    shaftHovered = true;
                }
            }
            else if (shaftHovered)
            {
                shaftHovered = false;
            }

            if (thumbClicked)
            {
                thumbRect.Y = e.Y - clickedThumbPos;

                if (thumbRect.Top < shaftRect.Top)
                {
                    thumbRect.Y = shaftRect.Top;
                }
                else if (thumbRect.Bottom > shaftRect.Bottom)
                {
                    thumbRect.Y = shaftRect.Bottom - thumbRect.Height;
                }

                Refresh(); // Need to redraw very quickly here. Refresh() rather than Invalidate().
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (upButtonHovered)
            {
                upButtonHovered = false;
                this.Invalidate();
            }

            if (downButtonHovered)
            {
                downButtonHovered = false;
                this.Invalidate();
            }

            if (thumbHovered)
            {
                thumbHovered = false;
                this.Invalidate();
            }

            if (shaftHovered)
            {
                shaftHovered = false;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (upButtonRect.Contains(e.Location))
            {
                upButtonClicked = true;
                this.Invalidate();
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
            else if (downButtonRect.Contains(e.Location))
            {
                downButtonClicked = true;
                this.Invalidate();
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
            else if (ModifierKeys.HasFlag(Keys.Shift) && shaftRect.Contains(e.Location))
            {
                thumbRect.Y = int.Clamp(
                    e.Y - (thumbRect.Height / 2),
                    shaftRect.Top,
                    shaftRect.Bottom - thumbRect.Height);

                this.Invalidate();
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));

                thumbClicked = true;
                clickedThumbPos = e.Y - thumbRect.Top;
            }
            else if (thumbRect.Contains(e.Location))
            {
                thumbClicked = true;
                clickedThumbPos = e.Y - thumbRect.Top;
                this.Invalidate();
            }
            else if (shaftRect.Contains(e.Location))
            {
                shaftClicked = true;
                clickedShaftPos = e.Y;
                scrollDirection = (e.Y < thumbRect.Top) ? -1 : 1;
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (thumbClicked)
            {
                OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, this.Value));
            }

            upButtonClicked = false;
            downButtonClicked = false;
            thumbClicked = false;
            shaftClicked = false;

            scrollDirection = 0;

            arrowTimer.Enabled = false;
            arrowTimer.Interval = 500;

            this.Invalidate();
        }

        protected override void OnRender(IDeviceContext deviceContext, RectFloat clipRect)
        {
            base.OnRender(deviceContext, clipRect);

            Point2Float[] upArrow =
            {
                new Point2Int32(upButtonRect.Width / 2, upButtonRect.Height / 3),
                new Point2Int32(upButtonRect.Width * 4 / 5, upButtonRect.Height * 2 / 3),
                new Point2Int32(upButtonRect.Width / 5, upButtonRect.Height * 2 / 3),
            };

            Point2Float[] downArrow =
            {
                new Point2Int32(downButtonRect.Width / 5 + 1, downButtonRect.Top + downButtonRect.Height / 3),
                new Point2Int32(downButtonRect.Width * 4 / 5, downButtonRect.Top + downButtonRect.Height / 3),
                new Point2Int32(downButtonRect.Width / 2, downButtonRect.Top - 1 + downButtonRect.Height * 2 / 3)
            };

            using (ISolidColorBrush brush = deviceContext.CreateSolidColorBrush(shaftColor))
            {
                deviceContext.FillRectangle(clipRect, brush);

                brush.Color = upButtonClicked ? clickedColor : upButtonHovered ? hoveredColor : normalColor;
                deviceContext.FillPolygon(upArrow, brush);

                brush.Color = downButtonClicked ? clickedColor : downButtonHovered ? hoveredColor : normalColor;
                deviceContext.FillPolygon(downArrow, brush);

                brush.Color = thumbClicked ? clickedColor : thumbHovered ? hoveredColor : normalColor;
                int padding = thumbRect.Width / 4;
                RectInt32 posRect = RectInt32.FromEdges(thumbRect.Left + padding, thumbRect.Top, thumbRect.Right - padding, thumbRect.Bottom);
                RoundedRect posRoundedRect = new RoundedRect(posRect, padding);
                deviceContext.FillRoundedRectangle(posRoundedRect, brush);
            }

            float dpiY = (float)UIScaleFactor.Current.Scale;

            using (ISolidColorBrush caretBrush = deviceContext.CreateSolidColorBrush(caretColor))
            {
                float curLineVPos = (float)(caret + 0) / maximum * shaftRect.Height + shaftRect.Top;
                curLineVPos = float.Clamp(curLineVPos, shaftRect.Top * dpiY, shaftRect.Bottom * dpiY);
                deviceContext.DrawLine(shaftRect.Left, curLineVPos, shaftRect.Right, curLineVPos, caretBrush, 2f * dpiY);
            }

            using (ISolidColorBrush indicatorPen = deviceContext.CreateSolidColorBrush(matchColor))
            {
                float strokeWidth = 4f * dpiY;

                indicatorPen.Color = bookmarkColor;
                foreach (int bookmark in this.bookmarks)
                {
                    float bkmkVPos = (float)bookmark / maximum * shaftRect.Height + shaftRect.Top;
                    bkmkVPos = float.Clamp(bkmkVPos, shaftRect.Top, shaftRect.Bottom);
                    deviceContext.DrawLine(shaftRect.Left + 6f * dpiY, bkmkVPos, shaftRect.Right - 6f * dpiY, bkmkVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = matchColor;
                foreach (int match in this.matches)
                {
                    float matchLineVPos = (float)match / maximum * shaftRect.Height + shaftRect.Top;
                    matchLineVPos = float.Clamp(matchLineVPos, shaftRect.Top, shaftRect.Bottom);
                    deviceContext.DrawLine(shaftRect.Left, matchLineVPos, shaftRect.Left + 4f * dpiY, matchLineVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = warningColor;
                foreach (int error in this.warnings)
                {
                    float warnLineVPos = (float)error / maximum * shaftRect.Height + shaftRect.Top;
                    warnLineVPos = float.Clamp(warnLineVPos, shaftRect.Top, shaftRect.Bottom);
                    deviceContext.DrawLine(shaftRect.Right - 4f * dpiY, warnLineVPos, shaftRect.Right, warnLineVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = errorColor;
                foreach (int error in this.errors)
                {
                    float errLineVPos = (float)error / maximum * shaftRect.Height + shaftRect.Top;
                    errLineVPos = float.Clamp(errLineVPos, shaftRect.Top, shaftRect.Bottom);
                    deviceContext.DrawLine(shaftRect.Right - 4f * dpiY, errLineVPos, shaftRect.Right, errLineVPos, indicatorPen, strokeWidth);
                }
            }
        }

        private void scrollByDelta()
        {
            if (arrowTimer.Enabled && arrowTimer.Interval != 10)
            {
                arrowTimer.Interval = 10;
            }

            int delta;
            ScrollEventType scrollType;
            if (upButtonClicked && upButtonHovered)
            {
                delta = -(int)float.Round(shaftRect.Height / (float)maximum);

                scrollType = ScrollEventType.SmallDecrement;
            }
            else if (downButtonClicked && downButtonHovered)
            {
                delta = (int)float.Round(shaftRect.Height / (float)maximum);
                scrollType = ScrollEventType.SmallIncrement;
            }
            else if (shaftClicked && shaftHovered)
            {
                if (thumbRect.Contains(thumbRect.Left, clickedShaftPos))
                {
                    OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, this.Value));
                    if (arrowTimer.Enabled)
                    {
                        arrowTimer.Enabled = false;
                    }
                    return;
                }

                if (scrollDirection == -1)
                {
                    delta = -thumbRect.Height;
                    scrollType = ScrollEventType.LargeDecrement;
                }
                else if (scrollDirection == 1)
                {
                    delta = thumbRect.Height;
                    scrollType = ScrollEventType.LargeIncrement;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            thumbRect.Y += delta;

            if (thumbRect.Top < shaftRect.Top)
            {
                thumbRect.Y = shaftRect.Top;
                scrollType = ScrollEventType.First;
            }
            else if (thumbRect.Bottom > shaftRect.Bottom)
            {
                thumbRect.Y = shaftRect.Bottom - thumbRect.Height;
                scrollType = ScrollEventType.Last;
            }

            Invalidate();
            OnScroll(new ScrollEventArgs(scrollType, this.Value));
        }
    }
}
