﻿using PaintDotNet.Controls;
using PaintDotNet.Direct2D1;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace PdnCodeLab
{
    internal sealed class IndicatorBar : Direct2DControl
    {
        private RectInt32 upButtonRect;
        private RectInt32 downButtonRect;
        private RectInt32 posSliderRect;
        private RectInt32 posTrackRect;

        private bool upButtonHover;
        private bool downButtonHover;
        private bool posSliderHover;
        private bool posTrackHover;

        private bool upButtonClick;
        private bool downButtonClick;
        private bool posSliderClick;
        private bool posTrackClick;

        private ColorRgb24 arrowColor;
        private ColorRgb24 arrowColorHover;
        private ColorRgb24 arrowColorClick;

        private ColorRgb24 posTrackColor;
        private ColorRgb24 posColor;
        private ColorRgb24 posColorHover;
        private ColorRgb24 posColorClick;

        private ColorRgb24 caretColor;
        private ColorRgb24 errorColor;
        private ColorRgb24 warningColor;
        private ColorRgb24 matchColor;
        private ColorRgb24 bookmarkColor;

        private int posClicked;
        private int posSliderClicked;
        private readonly Timer arrowTimer = new Timer();
        private int trackDirection;


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
                        arrowColor = new ColorRgb24(153, 153, 153);
                        arrowColorHover = new ColorRgb24(28, 151, 234);
                        arrowColorClick = new ColorRgb24(0, 122, 204);

                        posTrackColor = new ColorRgb24(62, 62, 66);
                        posColor = new ColorRgb24(104, 104, 104);
                        posColorHover = new ColorRgb24(158, 158, 158);
                        posColorClick = new ColorRgb24(239, 235, 239);

                        caretColor = SrgbColors.Gainsboro;
                        errorColor = new ColorRgb24(252, 62, 54);
                        warningColor = new ColorRgb24(149, 219, 125);
                        matchColor = SrgbColors.Orange;
                        bookmarkColor = SrgbColors.DeepSkyBlue;
                        break;

                    case Theme.Light:
                    default:
                        arrowColor = new ColorRgb24(134, 137, 153);
                        arrowColorHover = new ColorRgb24(28, 151, 234);
                        arrowColorClick = new ColorRgb24(0, 122, 204);

                        posTrackColor = new ColorRgb24(245, 245, 245);
                        posColor = new ColorRgb24(194, 195, 201);
                        posColorHover = new ColorRgb24(104, 104, 104);
                        posColorClick = new ColorRgb24(91, 91, 91);

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
                float scale = posTrackRect.Height / (float)maximum;
                return (int)MathF.Round((posSliderRect.Y - posTrackRect.Top) / scale);
            }
            set
            {
                float scale = posTrackRect.Height / (float)maximum;
                posSliderRect.Y = posTrackRect.Top + (int)MathF.Round(value * scale);
                if (posSliderRect.Top < posTrackRect.Top)
                {
                    posSliderRect.Y = posTrackRect.Top;
                }
                else if (posSliderRect.Bottom > posTrackRect.Bottom)
                {
                    posSliderRect.Y = posTrackRect.Bottom - posSliderRect.Height;
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
                    posSliderRect.Height = posTrackRect.Height;
                }
                else
                {
                    posSliderRect.Height = largeChange * posTrackRect.Height / value;
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
                    posSliderRect.Height = posTrackRect.Height;
                }
                else
                {
                    posSliderRect.Height = value * posTrackRect.Height / maximum;
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

            this.posSliderRect.Width = width;

            this.posTrackRect = RectInt32.FromEdges(this.ClientRectangle.Left, upButtonRect.Bottom + 1, this.ClientRectangle.Right, upButtonRect.Bottom + 10);

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

            posTrackRect.Height = downButtonRect.Top - upButtonRect.Bottom - 2;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (upButtonRect.Contains(e.Location))
            {
                if (!upButtonHover)
                {
                    upButtonHover = true;
                    this.Invalidate();
                }
            }
            else if (upButtonHover)
            {
                upButtonHover = false;
                this.Invalidate();
            }

            if (downButtonRect.Contains(e.Location))
            {
                if (!downButtonHover)
                {
                    downButtonHover = true;
                    this.Invalidate();
                }
            }
            else if (downButtonHover)
            {
                downButtonHover = false;
                this.Invalidate();
            }

            if (posSliderRect.Contains(e.Location))
            {
                if (!posSliderHover)
                {
                    posSliderHover = true;
                    this.Invalidate();
                }
            }
            else if (posSliderHover)
            {
                posSliderHover = false;
                this.Invalidate();
            }

            if (!posSliderRect.Contains(e.Location) && posTrackRect.Contains(e.Location))
            {
                if (trackDirection == -1 && e.Y > posSliderRect.Top)
                {
                    trackDirection = 0;
                }
                else if (trackDirection == 1 && e.Y < posSliderRect.Bottom)
                {
                    trackDirection = 0;
                }

                if (!posTrackHover)
                {
                    posTrackHover = true;
                }
            }
            else if (posTrackHover)
            {
                posTrackHover = false;
            }

            if (posSliderClick)
            {
                posSliderRect.Y = e.Y - posSliderClicked;

                if (posSliderRect.Top < posTrackRect.Top)
                {
                    posSliderRect.Y = posTrackRect.Top;
                }
                else if (posSliderRect.Bottom > posTrackRect.Bottom)
                {
                    posSliderRect.Y = posTrackRect.Bottom - posSliderRect.Height;
                }

                Refresh(); // Need to redraw very quickly here. Refresh() rather than Invalidate().
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, this.Value));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (upButtonHover)
            {
                upButtonHover = false;
                this.Invalidate();
            }

            if (downButtonHover)
            {
                downButtonHover = false;
                this.Invalidate();
            }

            if (posSliderHover)
            {
                posSliderHover = false;
                this.Invalidate();
            }

            if (posTrackHover)
            {
                posTrackHover = false;
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
                upButtonClick = true;
                this.Invalidate();
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
            else if (downButtonRect.Contains(e.Location))
            {
                downButtonClick = true;
                this.Invalidate();
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
            else if (posSliderRect.Contains(e.Location))
            {
                posSliderClick = true;
                posSliderClicked = e.Y - posSliderRect.Top;
                this.Invalidate();
            }
            else if (posTrackRect.Contains(e.Location))
            {
                posTrackClick = true;
                posClicked = e.Y;
                trackDirection = (e.Y < posSliderRect.Top) ? -1 : 1;
                scrollByDelta();
                arrowTimer.Enabled = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (posSliderClick)
            {
                OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, this.Value));
            }

            upButtonClick = false;
            downButtonClick = false;
            posSliderClick = false;
            posTrackClick = false;

            trackDirection = 0;

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

            using (ISolidColorBrush brush = deviceContext.CreateSolidColorBrush(posTrackColor))
            {
                deviceContext.FillRectangle(clipRect, brush);

                brush.Color = upButtonClick ? arrowColorClick : upButtonHover ? arrowColorHover : arrowColor;
                deviceContext.FillPolygon(upArrow, brush);

                brush.Color = downButtonClick ? arrowColorClick : downButtonHover ? arrowColorHover : arrowColor;
                deviceContext.FillPolygon(downArrow, brush);

                brush.Color = posSliderClick ? posColorClick : posSliderHover ? posColorHover : posColor;
                RectInt32 posRect = new RectInt32(posSliderRect.Width / 4, posSliderRect.Y, posSliderRect.Width - posSliderRect.Width / 2, posSliderRect.Height);
                deviceContext.FillRectangle(posRect, brush);
            }

            float dpiY = deviceContext.Dpi.Y / 96f;

            using (ISolidColorBrush caretBrush = deviceContext.CreateSolidColorBrush(caretColor))
            {
                float curLineVPos = (float)(caret + 0) / maximum * posTrackRect.Height + posTrackRect.Top;
                curLineVPos = Math.Clamp(curLineVPos, posTrackRect.Top * dpiY, posTrackRect.Bottom * dpiY);
                deviceContext.DrawLine(posTrackRect.Left, curLineVPos, posTrackRect.Right, curLineVPos, caretBrush, 2f * dpiY);
            }

            using (ISolidColorBrush indicatorPen = deviceContext.CreateSolidColorBrush(matchColor))
            {
                float strokeWidth = 4f * dpiY;

                indicatorPen.Color = bookmarkColor;
                foreach (int bookmark in this.bookmarks)
                {
                    float bkmkVPos = (float)bookmark / maximum * posTrackRect.Height + posTrackRect.Top;
                    bkmkVPos = Math.Clamp(bkmkVPos, posTrackRect.Top, posTrackRect.Bottom);
                    deviceContext.DrawLine(posTrackRect.Left + 6f * dpiY, bkmkVPos, posTrackRect.Right - 6f * dpiY, bkmkVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = matchColor;
                foreach (int match in this.matches)
                {
                    float matchLineVPos = (float)match / maximum * posTrackRect.Height + posTrackRect.Top;
                    matchLineVPos = Math.Clamp(matchLineVPos, posTrackRect.Top, posTrackRect.Bottom);
                    deviceContext.DrawLine(posTrackRect.Left, matchLineVPos, posTrackRect.Left + 4f * dpiY, matchLineVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = warningColor;
                foreach (int error in this.warnings)
                {
                    float warnLineVPos = (float)error / maximum * posTrackRect.Height + posTrackRect.Top;
                    warnLineVPos = Math.Clamp(warnLineVPos, posTrackRect.Top, posTrackRect.Bottom);
                    deviceContext.DrawLine(posTrackRect.Right - 4f * dpiY, warnLineVPos, posTrackRect.Right, warnLineVPos, indicatorPen, strokeWidth);
                }

                indicatorPen.Color = errorColor;
                foreach (int error in this.errors)
                {
                    float errLineVPos = (float)error / maximum * posTrackRect.Height + posTrackRect.Top;
                    errLineVPos = Math.Clamp(errLineVPos, posTrackRect.Top, posTrackRect.Bottom);
                    deviceContext.DrawLine(posTrackRect.Right - 4f * dpiY, errLineVPos, posTrackRect.Right, errLineVPos, indicatorPen, strokeWidth);
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
            if (upButtonClick && upButtonHover)
            {
                delta = -(int)MathF.Round(posTrackRect.Height / (float)maximum);

                scrollType = ScrollEventType.SmallDecrement;
            }
            else if (downButtonClick && downButtonHover)
            {
                delta = (int)MathF.Round(posTrackRect.Height / (float)maximum);
                scrollType = ScrollEventType.SmallIncrement;
            }
            else if (posTrackClick && posTrackHover)
            {
                if (posSliderRect.Contains(posSliderRect.Left, posClicked))
                {
                    OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, this.Value));
                    if (arrowTimer.Enabled)
                    {
                        arrowTimer.Enabled = false;
                    }
                    return;
                }

                if (trackDirection == -1)
                {
                    delta = -posSliderRect.Height;
                    scrollType = ScrollEventType.LargeDecrement;
                }
                else if (trackDirection == 1)
                {
                    delta = posSliderRect.Height;
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

            posSliderRect.Y += delta;

            if (posSliderRect.Top < posTrackRect.Top)
            {
                posSliderRect.Y = posTrackRect.Top;
                scrollType = ScrollEventType.First;
            }
            else if (posSliderRect.Bottom > posTrackRect.Bottom)
            {
                posSliderRect.Y = posTrackRect.Bottom - posSliderRect.Height;
                scrollType = ScrollEventType.Last;
            }

            Invalidate();
            OnScroll(new ScrollEventArgs(scrollType, this.Value));
        }
    }
}
