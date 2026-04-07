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
using PaintDotNet.Controls;
using PaintDotNet.Direct2D1;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace PdnCodeLab
{
    [DefaultEvent(nameof(ValueChanged))]
    [DefaultProperty(nameof(Color))]
    public partial class PdnColor : UserControl
    {
        private bool mouseDown;
        private bool ignore;
        private bool showAlpha;
        private float MasterHue;
        private float MasterSat;
        private float MasterVal;
        private byte MasterAlpha;
        private Bitmap wheelBmp;
        const int wheelPadding = 10;

        public PdnColor()
        {
            InitializeComponent();

            hColorSlider.Colors = Enumerable.Range(0, 65)
                .Select(i =>
                {
                    ColorHsv96Float hsv = new ColorHsv96Float(i / 65f * ColorHsv96Float.HueMaxValue, ColorHsv96Float.SaturationMaxValue, ColorHsv96Float.ValueMaxValue);
                    return (SrgbColorA)hsv.ToRgb();
                })
                .ToArray();
        }

        #region Control Properties
        [Category(nameof(CategoryAttribute.Data))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Color
        {
            get => GdiColorFromHsv(MasterHue, MasterSat, MasterVal, MasterAlpha);

            set
            {
                ColorHsv96Float hsv = HsvFromGdiColor(value);
                MasterHue = hsv.Hue;
                MasterSat = hsv.Saturation;
                MasterVal = hsv.Value;
                MasterAlpha = value.A;
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
            int wheelSize = colorWheelBox.ClientSize.Width - (wheelPadding * 2);

            #region create wheel
            GraphicsPath wheel_path = new GraphicsPath();
            Rectangle wheelRect = new Rectangle(wheelPadding, wheelPadding, wheelSize, wheelSize);
            wheel_path.AddEllipse(wheelRect);
            wheel_path.Flatten();

            float num_pts = wheel_path.PointCount;
            Color[] surround_colors = new Color[wheel_path.PointCount];
            for (int i = 0; i < num_pts; i++)
            {
                surround_colors[i] = GdiColorFromHsv(
                    ColorHsv96Float.HueMaxValue * i / num_pts,
                    ColorHsv96Float.SaturationMaxValue,
                    ColorHsv96Float.ValueMaxValue);
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

                float dpiX = g.DpiX / 96f;
                float dpiY = g.DpiY / 96f;

                //set _hue and sat marker
                float center = colorWheelBox.ClientSize.Width / 2f;
                float radius = MasterSat / ColorHsv96Float.SaturationMaxValue * (center - 1 - wheelPadding);

                float markerSize = 8 * dpiX;
                RectangleF markRect = new RectangleF
                {
                    X = center + radius * MathF.Cos(MasterHue / ColorHsv96Float.HueMaxValue * MathF.PI / .5f) - (markerSize / 2),
                    Y = center + radius * MathF.Sin(MasterHue / ColorHsv96Float.HueMaxValue * MathF.PI / .5f) - (markerSize / 2),
                    Width = markerSize,
                    Height = markerSize
                };

                Color markColor = GdiColorFromHsv(MasterHue, MasterSat, ColorHsv96Float.ValueMaxValue);
                using (SolidBrush markBrush = new SolidBrush(markColor))
                {
                    g.FillEllipse(markBrush, markRect);
                }

                using (Pen markPen = new Pen(Color.White, 1 * dpiX))
                {
                    g.DrawEllipse(markPen, markRect.X + 1 * dpiX, markRect.Y + 1 * dpiX, markRect.Width - 2 * dpiX, markRect.Height - 2 * dpiX);
                    markPen.Color = Color.Black;
                    g.DrawEllipse(markPen, markRect);
                }

                //draw color sample
                g.SmoothingMode = SmoothingMode.None;
                Color _colorVal = GdiColorFromHsv(MasterHue, MasterSat, MasterVal, MasterAlpha);

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

            // need to take Padding into account for the mouse XY and the center point
            float center = colorWheelBox.ClientSize.Width / 2f - wheelPadding;
            PointF offsetPoint = new PointF(e.X - wheelPadding - center, e.Y - wheelPadding - center);

            float rad = MathF.Atan2(offsetPoint.Y, offsetPoint.X) * .5f / MathF.PI;
            MasterHue = ColorHsv96Float.HueMaxValue * ((rad < 0) ? rad + 1 : rad);

            float offset = MathF.Sqrt(offsetPoint.Y * offsetPoint.Y + offsetPoint.X * offsetPoint.X) / center;
            MasterSat = ColorHsv96Float.SaturationMaxValue * ((offset > 1) ? 1 : offset);

            MasterVal = ColorHsv96Float.ValueMaxValue;

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

            ColorRgba32 rgba = ColorRgba32.FromRgba((byte)redBox.Value, (byte)greenBox.Value, (byte)blueBox.Value, (byte)alphaBox.Value);
            ColorHsv96Float hsv = HsvFromRgb(rgba.R, rgba.G, rgba.B);

            MasterHue = hsv.Hue;
            MasterSat = hsv.Saturation;
            MasterVal = hsv.Value;
            MasterAlpha = rgba.A;

            UpdateColorSliders();

            setColors(false, false);

            ignore = true;
            hexBox.Text = (showAlpha) ?
                rgba.Rgba.ToString("X8") :
                rgba.Rgba.ToString("X8").Substring(2);
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

            ColorRgba32 rgba = ColorRgba32.FromRgba((byte)rColorSlider.Value, (byte)gColorSlider.Value, (byte)bColorSlider.Value, (byte)aColorSlider.Value);
            ColorHsv96Float hsv = HsvFromRgb(rgba.R, rgba.G, rgba.B);

            MasterHue = hsv.Hue;
            MasterSat = hsv.Saturation;
            MasterVal = hsv.Value;
            MasterAlpha = rgba.A;

            setColors(false, false);

            ignore = true;
            redBox.Value = rgba.R;
            greenBox.Value = rgba.G;
            blueBox.Value = rgba.B;
            hexBox.Text = (showAlpha) ?
                rgba.Rgba.ToString("X8") :
                rgba.Rgba.ToString("X8").Substring(2);
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

            ColorRgba32 rgbaFromHex;
            try
            {
                uint value = Convert.ToUInt32(hexBox.Text, 16);
                rgbaFromHex = ColorRgba32.FromUInt32(value);
            }
            catch
            {
                ColorRgba32 rgbaFromHsv = RgbaFromHsv(MasterHue, MasterSat, MasterVal, MasterAlpha);
                hexBox.Text = (showAlpha) ?
                    rgbaFromHsv.Rgba.ToString("X8") :
                    rgbaFromHsv.Rgba.ToString("X8").Substring(2);

                return;
            }

            ColorHsv96Float hsv = HsvFromRgb(rgbaFromHex.R, rgbaFromHex.G, rgbaFromHex.B);

            MasterHue = hsv.Hue;
            MasterSat = hsv.Saturation;
            MasterVal = hsv.Value;
            MasterAlpha = (showAlpha) ? rgbaFromHex.A : byte.MaxValue;

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

            MasterHue = (float)hueBox.Value;
            MasterSat = (float)satBox.Value;
            MasterVal = (float)valBox.Value;
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

            MasterHue = hColorSlider.Value;
            MasterSat = sColorSlider.Value;
            MasterVal = vColorSlider.Value;
            MasterAlpha = (byte)aColorSlider.Value;

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

            if (rgb || hex)
            {
                ColorRgba32 rgbaColor = RgbaFromHsv(MasterHue, MasterSat, MasterVal, MasterAlpha);

                if (rgb)
                {
                    redBox.Value = rgbaColor.R;
                    greenBox.Value = rgbaColor.G;
                    blueBox.Value = rgbaColor.B;
                }

                if (hex)
                {
                    hexBox.Text = (showAlpha) ?
                        rgbaColor.Rgba.ToString("X8") :
                        rgbaColor.Rgba.ToString("X8").Substring(2);
                }
            }

            alphaBox.Value = MasterAlpha;
            hueBox.Value = (decimal)MasterHue;
            satBox.Value = (decimal)MasterSat;
            valBox.Value = (decimal)MasterVal;

            ignore = false;
        }

        private void UpdateColorSliders()
        {
            ignore = true;
            ColorRgb24 RGB = ColorRgb24.FromRgb((byte)redBox.Value, (byte)greenBox.Value, (byte)blueBox.Value);
            aColorSlider.Colors = new SrgbColorA[] { SrgbColors.Transparent, (SrgbColorA)ColorRgb24.FromRgb(RGB.R, RGB.G, RGB.B) };
            rColorSlider.Colors = new SrgbColorA[] { (SrgbColorA)ColorRgb24.FromRgb(byte.MinValue, RGB.G, RGB.B), (SrgbColorA)ColorRgb24.FromRgb(byte.MaxValue, RGB.G, RGB.B) };
            gColorSlider.Colors = new SrgbColorA[] { (SrgbColorA)ColorRgb24.FromRgb(RGB.R, byte.MinValue, RGB.B), (SrgbColorA)ColorRgb24.FromRgb(RGB.R, byte.MaxValue, RGB.B) };
            bColorSlider.Colors = new SrgbColorA[] { (SrgbColorA)ColorRgb24.FromRgb(RGB.R, RGB.G, byte.MinValue), (SrgbColorA)ColorRgb24.FromRgb(RGB.R, RGB.G, byte.MaxValue) };

            ColorRgb96Float minSaturation = new ColorHsv96Float(MasterHue, ColorHsv96Float.SaturationMinValue, MasterVal).ToRgb();
            ColorRgb96Float maxSaturation = new ColorHsv96Float(MasterHue, ColorHsv96Float.SaturationMaxValue, MasterVal).ToRgb();
            sColorSlider.Colors = new SrgbColorA[] { (SrgbColorA)minSaturation, (SrgbColorA)maxSaturation };

            ColorRgb96Float minValue = new ColorHsv96Float(MasterHue, MasterSat, ColorHsv96Float.ValueMinValue).ToRgb();
            ColorRgb96Float maxValue = new ColorHsv96Float(MasterHue, MasterSat, ColorHsv96Float.ValueMaxValue).ToRgb();
            vColorSlider.Colors = new SrgbColorA[] { (SrgbColorA)minValue, (SrgbColorA)maxValue };

            aColorSlider.Value = MasterAlpha;
            rColorSlider.Value = RGB.R;
            gColorSlider.Value = RGB.G;
            bColorSlider.Value = RGB.B;
            hColorSlider.Value = MasterHue;
            sColorSlider.Value = MasterSat;
            vColorSlider.Value = MasterVal;
            ignore = false;
        }
        #endregion

        private static Color GdiColorFromHsv(float hue, float saturation, float value, byte alpha = byte.MaxValue)
        {
            ColorRgba32 rgb = RgbaFromHsv(hue, saturation, value, alpha);
            return Color.FromArgb(rgb.A, rgb.R, rgb.G, rgb.B);
        }

        private static ColorRgba32 RgbaFromHsv(float hue, float saturation, float value, byte alpha = byte.MaxValue)
        {
            ColorRgb96Float rgbFloat = new ColorHsv96Float(hue, saturation, value).ToRgb();
            return ColorRgba32.FromRgba((byte)(rgbFloat.R * 255f), (byte)(rgbFloat.G * 255f), (byte)(rgbFloat.B * 255f), alpha);
        }

        private static ColorHsv96Float HsvFromGdiColor(Color gdiColor)
        {
            return HsvFromRgb(gdiColor.R, gdiColor.G, gdiColor.B);
        }

        private static ColorHsv96Float HsvFromRgb(byte r, byte g, byte b)
        {
            ColorRgb96Float rgb = new ColorRgb96Float(r / 255f, g / 255f, b / 255f);
            return ColorHsv96Float.FromRgb(rgb);
        }
    }

    [DefaultEvent(nameof(ValueChanged))]
    [DefaultProperty(nameof(Value))]
    public class ColorSlider : Direct2DControl
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
            set => this.maxValue = Math.Max(value, 1);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal SrgbColorA[] Colors
        {
            get => this.colors;
            set
            {
                this.colors = value;
                this.Refresh();
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
        private SrgbColorA[] colors = { SrgbColors.White, SrgbColors.Black };
        private bool isMouseOver;
        private bool isMouseDown;
        private const int sliderPadding = 4;

        public ColorSlider()
        {
            this.Width = 73;
            this.Height = 15;
        }

        protected override void OnRender(PaintDotNet.Direct2D1.IDeviceContext deviceContext, RectFloat clipRect)
        {
            base.OnRender(deviceContext, clipRect);

            Rectangle clientRect = this.ClientRectangle;
            float padding = UIUtil.Scale(sliderPadding);

            RectFloat colorRect = RectFloat.FromEdges(
                clientRect.Left + padding,
                clientRect.Top,
                clientRect.Right - padding,
                clientRect.Bottom  - padding);

            // Draw color band
            float totalColors = colors.Length - 1;
            GradientStop[] gradientStopArray = colors
                .Select((c, i) => new GradientStop(i / totalColors, (ColorRgba128Float)c))
                .ToArray();

            ReadOnlySpan<GradientStop> gradientStops = new ReadOnlySpan<GradientStop>(gradientStopArray);

            using (IGradientStopCollection gradientStopCollection = deviceContext.CreateGradientStopCollection(gradientStops))
            using (ILinearGradientBrush gradientBrush = deviceContext.CreateLinearGradientBrush(colorRect.TopLeft, colorRect.TopRight, gradientStopCollection))
            {
                deviceContext.FillRectangle(colorRect, gradientBrush);
            }

            // Draw value marker
            float markPos = value / maxValue * colorRect.Width + padding;
            Point2Float[] markerPolygon =
            {
                new Point2Float(markPos, clientRect.Bottom - UIUtil.Scale(7)),  // top
                new Point2Float(markPos - UIUtil.Scale(3), clientRect.Bottom),  // left
                new Point2Float(markPos + UIUtil.Scale(3), clientRect.Bottom)   // right
            };

            SrgbColorA markerColor = (isMouseOver) ? SrgbColors.Blue : (SrgbColorA)this.ForeColor;
            using (ISolidColorBrush markerFillBrush = deviceContext.CreateSolidColorBrush(markerColor))
            using (ISolidColorBrush markerDrawBrush = deviceContext.CreateSolidColorBrush(this.BackColor))
            {
                deviceContext.DrawPolygon(markerPolygon, markerDrawBrush);
                deviceContext.FillPolygon(markerPolygon, markerFillBrush);
            }
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

            float padding = UIUtil.Scale(sliderPadding);
            float range = this.ClientSize.Width - (padding * 2f);

            float newValue = (e.X - padding) / range * maxValue;
            value = Math.Clamp(newValue, 0, maxValue);
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
    }
}
