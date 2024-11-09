using PaintDotNet;
using PaintDotNet.Controls;
using PaintDotNet.Direct2D1;
using PaintDotNet.DirectWrite;
using PaintDotNet.Rendering;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace PdnCodeLab
{
    public class MessageLabel : Direct2DControl
    {
        [DefaultValue(MessageType.None)]
        [Category(nameof(CategoryAttribute.Appearance))]
        public MessageType MessageType { get; set; }

        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public MessageLabel()
        {
            this.AllowHardwareRendering = false;
        }

        protected override void OnRender(IDeviceContext deviceContext, RectFloat clipRect)
        {
            base.OnRender(deviceContext, clipRect);

            using ISolidColorBrush colorBrush = deviceContext.CreateSolidColorBrush(this.ForeColor);

            TextMeasurement headerMeasurement = default;
            IDirectWriteFactory dwFactory = this.DirectWriteFactory;

            if (this.MessageType != MessageType.None)
            {
                string headerText = this.MessageType switch
                {
                    MessageType.Info => "ℹ️ Info",
                    MessageType.Warning => "⚠️ Warning",
                    MessageType.Error => "🛑 Error",
                    _ => throw new NotImplementedException()
                };

                using ITextFormat headerTextFormat = dwFactory.CreateTextFormat(
                    this.Font.Name,
                    null,
                    FontWeight.Normal,
                    FontStyle.Normal,
                    FontStretch.Normal,
                    UIScaleFactor.Current.ConvertFontPointsToPixels(this.Font.SizeInPoints + 5));

                using ITextLayout headerLayout = dwFactory.CreateTextLayout(headerText, headerTextFormat, clipRect.Width);

                headerMeasurement = headerLayout.Measure();

                deviceContext.DrawTextLayout(Point2Float.Zero, headerLayout, colorBrush);
            }

            using ITextFormat textFormat = dwFactory.CreateTextFormat(
                this.Font.Name,
                null,
                this.Font.Bold ? FontWeight.Bold : FontWeight.Normal,
                this.Font.Italic ? FontStyle.Italic : FontStyle.Normal,
                FontStretch.Normal,
                UIScaleFactor.Current.ConvertFontPointsToPixels(this.Font.SizeInPoints));

            RectFloat textRect = (headerMeasurement.Height > 0)
                ? RectFloat.FromEdges(clipRect.Left, clipRect.Top + headerMeasurement.Height + 5, clipRect.Right, clipRect.Right)
                : clipRect;

            deviceContext.DrawText(this.Text, textFormat, textRect, colorBrush);
        }
    }

    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error
    }
}
