/////////////////////////////////////////////////////////////////////////////////
// ColorWindow for CodeLab
// Copyright 2015 Rob Tauler
// Portions Copyright ©2016 BoltBait. All Rights Reserved.
// Portions Copyright ©2016 Jason Wendt. All Rights Reserved.
// Portions Copyright ©Microsoft Corporation. All Rights Reserved.
//
// THE DEVELOPERS MAKE NO WARRANTY OF ANY KIND REGARDING THE CODE. THEY
// SPECIFICALLY DISCLAIM ANY WARRANTY OF FITNESS FOR ANY PARTICULAR PURPOSE OR
// ANY OTHER WARRANTY.  THE CODELAB DEVELOPERS DISCLAIM ALL LIABILITY RELATING
// TO THE USE OF THIS CODE.  NO LICENSE, EXPRESS OR IMPLIED, BY ESTOPPEL OR
// OTHERWISE, TO ANY INTELLECTUAL PROPERTY RIGHTS IS GRANTED HEREIN.
//
// Latest distribution: https://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace PdnCodeLab
{
    [DefaultEvent(nameof(ValueChanged))]
    [DefaultProperty(nameof(Color))]
    public partial class PdnColor : UserControl
    {
        private bool mouseDown;
        private bool ignore;
        private bool showAlpha;
        private double MasterHue;
        private double MasterSat;
        private double MasterVal;
        private int MasterAlpha;
        private Bitmap wheelBmp;
        private readonly Color[] HsvRainbow;

        public PdnColor()
        {
            InitializeComponent();

            HsvRainbow = new Color[65];
            for (int i = 0; i < 65; i++)
            {
                HsvRainbow[i] = HSVColor.ToColor(255, i / 65f, 1, 1);
            }
        }

        #region Control Properties
        [Category(nameof(CategoryAttribute.Data))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Color
        {
            get => HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, MasterVal);
            set
            {
                Color _colorVal = value;
                MasterHue = HSVColor.FromColor(_colorVal, MasterHue).Hue;
                MasterSat = HSVColor.FromColor(_colorVal, MasterHue).Sat;
                MasterVal = HSVColor.FromColor(_colorVal, MasterHue).Value;
                MasterAlpha = _colorVal.A;
                setColors(true, true);
                UpdateColorSliders();
                colorWheelBox.Refresh();
                OnValueChanged();
            }
        }

        [Category(nameof(CategoryAttribute.Behavior))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowAlpha
        {
            get => showAlpha;
            set
            {
                showAlpha = value;
                if (showAlpha)
                {
                    headerPanel3.Visible = true;
                    opacityLabel.Visible = true;
                    alphaBox.Visible = true;
                    aColorSlider.Visible = true;
                }
                else
                {
                    headerPanel3.Visible = false;
                    opacityLabel.Visible = false;
                    alphaBox.Visible = false;
                    aColorSlider.Visible = false;
                }
            }
        }
        #endregion

        #region Event Handler
        [Category(nameof(CategoryAttribute.Action))]
        public event EventHandler ValueChanged;
        protected void OnValueChanged()
        {
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Color Wheel functions
        private void ColorWheel_Paint()
        {
            float Padding = colorWheelBox.ClientSize.Width * 3 / 210;
            float Radius = colorWheelBox.ClientSize.Width / 2 - Padding;

            #region create wheel
            GraphicsPath wheel_path = new GraphicsPath();
            RectangleF wheelRect = new RectangleF(Padding, Padding, Radius * 2, Radius * 2);
            wheel_path.AddEllipse(wheelRect);
            wheel_path.Flatten();

            float num_pts = wheel_path.PointCount;
            Color[] surround_colors = new Color[wheel_path.PointCount];
            for (int i = 0; i < num_pts; i++)
            {
                surround_colors[i] = HSVColor.ToColor(255, i / num_pts, 1, 1);
            }
            #endregion

            if (wheelBmp == null)
            {
                wheelBmp = new Bitmap(colorWheelBox.Width, colorWheelBox.Width);
            }

            using (Graphics g = Graphics.FromImage(wheelBmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (PathGradientBrush path_brush = new PathGradientBrush(wheel_path))
                {
                    path_brush.CenterColor = Color.White;
                    path_brush.SurroundColors = surround_colors;

                    g.FillPath(path_brush, wheel_path);
                    using (Pen thick_pen = new Pen(this.BackColor, 2.0f))
                    {
                        g.DrawPath(thick_pen, wheel_path);
                    }
                }

                //set _hue and sat marker
                double hlfht = Radius + Padding;
                double radius = MasterSat * (hlfht - 1 - Padding);

                PointF _huePoint = new PointF
                {
                    X = (float)(hlfht + radius * Math.Cos(MasterHue * Math.PI / .5) - Padding),
                    Y = (float)(hlfht + radius * Math.Sin(MasterHue * Math.PI / .5) - Padding)
                };
                SizeF _hueSize = new SizeF(Padding * 2, Padding * 2);
                RectangleF _hueMark = new RectangleF(_huePoint, _hueSize);
                using (SolidBrush markBrush = new SolidBrush(HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, 1)))
                {
                    g.FillEllipse(markBrush, _hueMark);
                }

                float dpiX = g.DpiX / 96f;
                float dpiY = g.DpiY / 96f;

                using (Pen markPen = new Pen(Color.White, 1 * dpiX))
                {
                    g.DrawEllipse(markPen, _hueMark.X + 1 * dpiX, _hueMark.Y + 1 * dpiX, _hueMark.Width - 2 * dpiX, _hueMark.Height - 2 * dpiX);
                    markPen.Color = Color.Black;
                    g.DrawEllipse(markPen, _hueMark);
                }

                //draw color sample
                g.SmoothingMode = SmoothingMode.None;
                Color _colorVal = HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, MasterVal);

                Rectangle SwatchRect1 = new Rectangle(0, 0, (int)MathF.Round(30 * dpiX), (int)MathF.Round(30 * dpiY));
                Rectangle SwatchRect2 = Rectangle.FromLTRB(SwatchRect1.Left + (int)MathF.Round(1 * dpiX), SwatchRect1.Top + (int)MathF.Round(1 * dpiY), SwatchRect1.Right - (int)MathF.Round(1 * dpiX), SwatchRect1.Bottom - (int)MathF.Round(1 * dpiY));

                using (HatchBrush hb = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.White))
                {
                    g.FillRectangle(hb, SwatchRect1);
                }
                using (SolidBrush SB = new SolidBrush(_colorVal))
                {
                    g.FillRectangle(SB, SwatchRect1);
                }
                using (Pen outlinePen = new Pen(Color.Black, (int)MathF.Round(1 * dpiX)))
                {
                    outlinePen.Alignment = PenAlignment.Inset;

                    g.DrawRectangle(outlinePen, SwatchRect1);

                    outlinePen.Color = Color.White;
                    g.DrawRectangle(outlinePen, SwatchRect2);
                }
            }
        }

        private void ColorWheel_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            ColorWheel_MouseMove(sender, e);
        }

        private void ColorWheel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
            {
                return;
            }

            float Padding = colorWheelBox.ClientSize.Width * 3 / 100;
            float Radius = colorWheelBox.ClientSize.Width / 2 - Padding;

            float hlfht = Radius + Padding;
            double offset = Math.Sqrt((e.Y - hlfht) * (e.Y - hlfht) + (e.X - hlfht) * (e.X - hlfht)) / hlfht;

            double rad = Math.Atan2(e.Y - hlfht, e.X - hlfht) * .5 / Math.PI;
            MasterHue = (rad < 0) ? rad + 1 : rad;
            MasterSat = (offset > 1) ? 1 : offset;
            MasterVal = 1;

            UpdateColorSliders();
            setColors(true, true);
            OnValueChanged();
            colorWheelBox.Refresh();
        }

        private void ColorWheel_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateColorSliders();
            OnValueChanged();
            mouseDown = false;
            colorWheelBox.Refresh();
        }

        private void colorWheel_Paint(object sender, PaintEventArgs e)
        {
            ColorWheel_Paint();
            e.Graphics.DrawImage(wheelBmp, 0, 0);
        }
        #endregion

        #region ARGB Controls functions
        private void rgb_ValueChanged()
        {
            if (ignore)
            {
                return;
            }

            Color _colorVal = Color.FromArgb((int)alphaBox.Value, (int)redBox.Value, (int)greenBox.Value, (int)blueBox.Value);
            MasterHue = HSVColor.FromColor(_colorVal, MasterHue).Hue;
            MasterSat = HSVColor.FromColor(_colorVal, MasterHue).Sat;
            MasterVal = HSVColor.FromColor(_colorVal, MasterHue).Value;
            MasterAlpha = _colorVal.A;

            UpdateColorSliders();

            setColors(false, false);

            ignore = true;
            hexBox.Text = (showAlpha) ?
                _colorVal.ToArgb().ToString("X8") :
                _colorVal.ToArgb().ToString("X8").Substring(2);
            ignore = false;

            colorWheelBox.Refresh();
            OnValueChanged();
        }

        private void ARGB_ValueChanged(object sender, EventArgs e)
        {
            rgb_ValueChanged();
        }

        private void ARGB_MouseUp(object sender, MouseEventArgs e)
        {
            rgb_ValueChanged();
        }

        private void ARGB_Leave(object sender, EventArgs e)
        {
            rgb_ValueChanged();
        }

        private void RGB_Sliders_ValueChanged(object sender, EventArgs e)
        {
            RGB_Sliders_ValueChanged();
        }

        private void RGB_Sliders_ValueChanged()
        {
            if (ignore)
            {
                return;
            }

            Color _colorVal = Color.FromArgb((int)aColorSlider.Value, (int)rColorSlider.Value, (int)gColorSlider.Value, (int)bColorSlider.Value);
            MasterHue = HSVColor.FromColor(_colorVal, MasterHue).Hue;
            MasterSat = HSVColor.FromColor(_colorVal, MasterHue).Sat;
            MasterVal = HSVColor.FromColor(_colorVal, MasterHue).Value;
            MasterAlpha = (int)aColorSlider.Value;

            setColors(false, false);

            ignore = true;
            redBox.Value = _colorVal.R;
            greenBox.Value = _colorVal.G;
            blueBox.Value = _colorVal.B;
            hexBox.Text = (showAlpha) ?
                _colorVal.ToArgb().ToString("X8") :
                _colorVal.ToArgb().ToString("X8").Substring(2);
            ignore = false;

            UpdateColorSliders();
            colorWheelBox.Refresh();
            OnValueChanged();
        }
        #endregion

        #region Hex Control functions
        private void hexBox_Changed()
        {
            if (ignore)
            {
                return;
            }

            Color _colorVal;
            try
            {
                ColorConverter c = new ColorConverter();
                _colorVal = (Color)c.ConvertFromString("#" + hexBox.Text);
            }
            catch
            {
                _colorVal = HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, MasterVal);
                hexBox.Text = (showAlpha) ?
                    _colorVal.ToArgb().ToString("X8") :
                    _colorVal.ToArgb().ToString("X8").Substring(2);

                return;
            }

            MasterHue = HSVColor.FromColor(_colorVal, MasterHue).Hue;
            MasterSat = HSVColor.FromColor(_colorVal, MasterHue).Sat;
            MasterVal = HSVColor.FromColor(_colorVal, MasterHue).Value;
            MasterAlpha = (showAlpha) ? _colorVal.A : 255;

            setColors(true, false);
            colorWheelBox.Refresh();
            UpdateColorSliders();

            OnValueChanged();
        }

        private void hexBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                hexBox_Changed();
            }
        }

        private void hexBox_Leave(object sender, EventArgs e)
        {
            hexBox_Changed();
        }

        private void Hex_MouseUp(object sender, MouseEventArgs e)
        {
            hexBox_Changed();
        }
        #endregion

        #region HSV Controls functions
        private void HSV_ValueChanged()
        {
            if (ignore)
            {
                return;
            }

            MasterHue = (double)hueBox.Value / 360;
            MasterSat = (double)satBox.Value / 100;
            MasterVal = (double)valBox.Value / 100;
            setColors(true, true);
            colorWheelBox.Refresh();
            UpdateColorSliders();
            OnValueChanged();
        }

        private void HSV_MouseUp(object sender, MouseEventArgs e)
        {
            HSV_ValueChanged();
        }

        private void HSV_Leave(object sender, EventArgs e)
        {
            HSV_ValueChanged();
        }

        private void HSV_ValueChanged(object sender, EventArgs e)
        {
            HSV_ValueChanged();
        }

        private void HSV_Sliders_ValueChanged()
        {
            if (ignore)
            {
                return;
            }

            MasterHue = hColorSlider.Value / 360f;
            MasterSat = sColorSlider.Value / 100f;
            MasterVal = vColorSlider.Value / 100f;
            MasterAlpha = (int)aColorSlider.Value;

            setColors(true, true);
            UpdateColorSliders();
            colorWheelBox.Refresh();
            OnValueChanged();
        }

        private void HSV_Sliders_ValueChanged(object sender, EventArgs e)
        {
            HSV_Sliders_ValueChanged();
        }
        #endregion

        #region Update controls with new Values
        private void setColors(bool rgb, bool hex)
        {
            ignore = true;
            Color _colorVal = HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, MasterVal);
            if (rgb)
            {
                redBox.Value = _colorVal.R;
                greenBox.Value = _colorVal.G;
                blueBox.Value = _colorVal.B;
            }
            alphaBox.Value = _colorVal.A;
            hueBox.Value = (decimal)MasterHue * 360;
            satBox.Value = (decimal)MasterSat * 100;
            valBox.Value = (decimal)MasterVal * 100;
            if (hex)
            {
                hexBox.Text = (showAlpha) ?
                    _colorVal.ToArgb().ToString("X8") :
                    _colorVal.ToArgb().ToString("X8").Substring(2);
            }
            ignore = false;
        }

        private void UpdateColorSliders()
        {
            // TODO: Updating everything together at the same time is probably not the most optimal. It's already super fast though...

            ignore = true;
            //Color RGB = HSVColor.ToColor(MasterAlpha, MasterHue, MasterSat, MasterVal);
            Color RGB = Color.FromArgb((int)redBox.Value, (int)greenBox.Value, (int)blueBox.Value);
            aColorSlider.Colors = new Color[] { Color.Transparent, Color.FromArgb(RGB.R, RGB.G, RGB.B) };
            rColorSlider.Colors = new Color[] { Color.FromArgb(byte.MinValue, RGB.G, RGB.B), Color.FromArgb(byte.MaxValue, RGB.G, RGB.B) };
            gColorSlider.Colors = new Color[] { Color.FromArgb(RGB.R, byte.MinValue, RGB.B), Color.FromArgb(RGB.R, byte.MaxValue, RGB.B) };
            bColorSlider.Colors = new Color[] { Color.FromArgb(RGB.R, RGB.G, byte.MinValue), Color.FromArgb(RGB.R, RGB.G, byte.MaxValue) };

            hColorSlider.Colors = HsvRainbow;

            Color minSaturation = HSVColor.ToColor(byte.MaxValue, MasterHue, 0, MasterVal);
            Color maxSaturation = HSVColor.ToColor(byte.MaxValue, MasterHue, 1, MasterVal);
            sColorSlider.Colors = new Color[] { minSaturation, maxSaturation };

            Color minValue = HSVColor.ToColor(byte.MaxValue, MasterHue, MasterSat, 0);
            Color maxValue = HSVColor.ToColor(byte.MaxValue, MasterHue, MasterSat, 1);
            vColorSlider.Colors = new Color[] { minValue, maxValue };

            aColorSlider.Value = MasterAlpha;
            rColorSlider.Value = RGB.R;
            gColorSlider.Value = RGB.G;
            bColorSlider.Value = RGB.B;
            hColorSlider.Value = (float)(MasterHue * 360);
            sColorSlider.Value = (float)(MasterSat * 100);
            vColorSlider.Value = (float)(MasterVal * 100);
            ignore = false;
        }
        #endregion

        private struct HSVColor
        {
            internal double Hue { get; set; }
            internal double Sat { get; set; }
            internal double Value { get; set; }

            internal static Color ToColor(int alpha, double h, double s, double v)
            {
                double r, g, b;
                if (s == 0)
                {
                    r = v;
                    g = v;
                    b = v;
                }
                else
                {
                    double varH = h * 6;
                    double varI = Math.Floor(varH);
                    double var1 = v * (1 - s);
                    double var2 = v * (1 - (s * (varH - varI)));
                    double var3 = v * (1 - (s * (1 - (varH - varI))));

                    if (varI == 0)
                    {
                        r = v;
                        g = var3;
                        b = var1;
                    }
                    else if (varI == 1)
                    {
                        r = var2;
                        g = v;
                        b = var1;
                    }
                    else if (varI == 2)
                    {
                        r = var1;
                        g = v;
                        b = var3;
                    }
                    else if (varI == 3)
                    {
                        r = var1;
                        g = var2;
                        b = v;
                    }
                    else if (varI == 4)
                    {
                        r = var3;
                        g = var1;
                        b = v;
                    }
                    else
                    {
                        r = v;
                        g = var1;
                        b = var2;
                    }
                }
                return Color.FromArgb(alpha, (int)(r * 255), (int)(g * 255), (int)(b * 255));
            }

            internal static HSVColor FromColor(Color c, double oldHue)
            {
                double r = c.R / 255.0;
                double g = c.G / 255.0;
                double b = c.B / 255.0;
                double varMin = Math.Min(r, Math.Min(g, b));
                double varMax = Math.Max(r, Math.Max(g, b));
                double delMax = varMax - varMin;
                HSVColor hsv = new HSVColor();

                hsv.Value = varMax;

                if (delMax == 0)
                {
                    hsv.Hue = oldHue;
                    hsv.Sat = 0;
                }
                else
                {
                    double delR = (((varMax - r) / 6) + (delMax / 2)) / delMax;
                    double delG = (((varMax - g) / 6) + (delMax / 2)) / delMax;
                    double delB = (((varMax - b) / 6) + (delMax / 2)) / delMax;

                    hsv.Sat = delMax / varMax;

                    if (r == varMax)
                    {
                        hsv.Hue = delB - delG;
                    }
                    else if (g == varMax)
                    {
                        hsv.Hue = (1.0 / 3) + delR - delB;
                    }
                    else //// if (b == varMax) 
                    {
                        hsv.Hue = (2.0 / 3) + delG - delR;
                    }

                    if (hsv.Hue < 0)
                    {
                        hsv.Hue++;
                    }

                    if (hsv.Hue > 1)
                    {
                        hsv.Hue--;
                    }
                }

                return hsv;
            }
        }
    }

    [DefaultEvent(nameof(ValueChanged))]
    [DefaultProperty(nameof(Value))]
    public class ColorSlider : PictureBox
    {
        #region Properties
        [Category(nameof(CategoryAttribute.Data))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Value
        {
            get => this.value;
            set
            {
                this.value = value;
                OnValueChanged();
                this.Refresh();
            }
        }

        [Category(nameof(CategoryAttribute.Behavior))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxValue
        {
            get => this.maxValue;
            set => this.maxValue = value;
        }

        [Category(nameof(CategoryAttribute.Appearance))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color[] Colors
        {
            get => this.colors;
            set
            {
                this.colors = value;
                DrawColors();
            }
        }
        #endregion

        #region Event handler
        [Category(nameof(CategoryAttribute.Action))]
        public event EventHandler ValueChanged;
        protected void OnValueChanged()
        {
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        private float value = 0;
        private int maxValue = byte.MaxValue;
        private Color[] colors = { Color.White, Color.Black };
        private Bitmap markerBmp;
        private bool isMouseOver;
        private bool isMouseDown;

        public ColorSlider()
        {
            this.Width = 73;
            this.Height = 15;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            isMouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!isMouseDown)
            {
                return;
            }

            float range = this.ClientSize.Width * 0.8904f;
            float offset = this.ClientSize.Width * 0.0548f;

            value = e.X / range * maxValue - offset;
            value = Math.Clamp(value, 0, maxValue);
            this.Refresh();
            OnValueChanged();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            isMouseDown = true;
            OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            isMouseOver = true;
            this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            isMouseOver = false;
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            DrawMarker();
            pe.Graphics.DrawImage(markerBmp, 0, 0);
        }

        private void DrawMarker()
        {
            if (this.markerBmp == null || this.markerBmp.Size != this.ClientSize)
            {
                this.markerBmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            using (Graphics g = Graphics.FromImage(markerBmp))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;

                // clear bitmap
                g.Clear(Color.Transparent);

                if (maxValue == 0)
                {
                    maxValue = 1;
                }

                float dpi = g.DpiX / 96f;
                float markPos = value / maxValue * (g.VisibleClipBounds.Width - (9 * dpi)) + (4 * dpi);

                PointF top = new PointF(markPos, g.VisibleClipBounds.Bottom - (7 * dpi));
                PointF left = new PointF(markPos - (3.5f * dpi), g.VisibleClipBounds.Bottom);
                PointF right = new PointF(markPos + (3.5f * dpi), g.VisibleClipBounds.Bottom);
                PointF[] marker = { top, left, right };

                Color markerColor = (isMouseOver) ? Color.Blue : Parent.ForeColor;

                using (SolidBrush markerBrush = new SolidBrush(markerColor))
                using (Pen markerPen = new Pen(this.BackColor, 1))
                {
                    if (isMouseOver)
                    {
                        g.FillPolygon(markerBrush, marker);
                        g.DrawPolygon(markerPen, marker);
                    }
                    else
                    {
                        g.DrawPolygon(markerPen, marker);
                        g.FillPolygon(markerBrush, marker);
                    }
                }
            }
        }

        private void DrawColors()
        {
            if (this.Image == null || this.Image.Size != this.ClientSize)
            {
                this.Image = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            }

            using (Graphics g = Graphics.FromImage(this.Image))
            {
                g.Clear(Color.Transparent);
                float dpi = g.DpiX / 96f;
                RectangleF colorRect = new RectangleF(g.VisibleClipBounds.X + (4 * dpi), g.VisibleClipBounds.Y, g.VisibleClipBounds.Width - (8 * dpi), g.VisibleClipBounds.Height - (4 * dpi));

                using (HatchBrush brush = new HatchBrush(HatchStyle.DiagonalCross, Color.LightGray, Color.White))
                {
                    g.FillRectangle(brush, colorRect);
                }

                // custom Rect to avoid a bug in LinearGradientBrush on HiDPI
                RectangleF gradientRect = new RectangleF(colorRect.Left - 1, colorRect.Top, colorRect.Width + 1, colorRect.Height);
                using (LinearGradientBrush brush = new LinearGradientBrush(gradientRect, colors[0], colors[1], LinearGradientMode.Horizontal))
                {
                    if (maxValue == 360)
                    {
                        ColorBlend cb = new ColorBlend();
                        float[] positions = new float[colors.Length];
                        positions[0] = 0;
                        for (int i = 1; i < colors.Length - 1; i++)
                        {
                            positions[i] = i / (float)colors.Length;
                        }
                        positions[colors.Length - 1] = 1;
                        cb.Positions = positions;
                        cb.Colors = colors;
                        brush.InterpolationColors = cb;
                    }

                    g.FillRectangle(brush, colorRect);
                }
            }
        }
    }
}
