using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace PaintDotNet.Effects
{
    internal static class ShapeBuilder
    {
        private static Size shapeSize;
        private static Point shapeOffset;
        private static SolidColorBrush strokeBrush;
        private static SolidColorBrush fillBrush;
        private static double strokeThickness;

        private const string InvalidShapeFormat = "Shape code is invalid or otherwise unrecognized.";
        private static string exceptionMsg;

        internal static string Exception => exceptionMsg;
        internal static System.Drawing.Bitmap Shape;

        internal static void SetEnviromentParams(int width, int height, int xOffset, int yOffset, ColorBgra strokeColor, ColorBgra fillColor, double strokeWidth)
        {
            shapeSize = new Size(width, height);
            shapeOffset = new Point(xOffset, yOffset);
            strokeBrush = new SolidColorBrush(Color.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B));
            fillBrush = new SolidColorBrush(Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B));
            strokeThickness = strokeWidth;
        }

        internal static bool RenderShape(string shapeCode)
        {
            exceptionMsg = null;

            if (string.IsNullOrWhiteSpace(shapeCode))
            {
                exceptionMsg = InvalidShapeFormat;
                return false;
            }

            XmlDocument xDoc = TryLoadXml(shapeCode);
            if (xDoc == null)
            {
                return false;
            }

            XmlElement docElement = xDoc.DocumentElement;

            if (docElement.Name == "Page")
            {
                Page page = TryParsePage(shapeCode);
                if (page != null)
                {
                    RenderPage(page);
                    return true;
                }
            }
            else if (docElement.Name == "ps:SimpleGeometryShape")
            {
                if (docElement.HasChildNodes)
                {
                    string outerXml = docElement.FirstChild.OuterXml;
                    int xmlnsStartIndex = outerXml.IndexOf(" xmlns=");
                    int xmlnsEndIndex = outerXml.IndexOf(">");

                    if (xmlnsStartIndex == -1 || xmlnsEndIndex == -1)
                    {
                        exceptionMsg = InvalidShapeFormat;
                        return false;
                    }

                    string pageCode = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
                        + outerXml.Substring(0, xmlnsStartIndex) + outerXml.Substring(xmlnsEndIndex)
                        + "</Page>";

                    Page page = TryParsePage(pageCode);
                    if (page?.Content is Geometry geometry)
                    {
                        RenderGeometry(geometry);
                        return true;
                    }
                }
                else if (docElement.HasAttributes && docElement.HasAttribute("Geometry"))
                {
                    string geometryCode = docElement.Attributes.GetNamedItem("Geometry").InnerText;
                    if (string.IsNullOrWhiteSpace(geometryCode))
                    {
                        exceptionMsg = "Can not find the Geometry attribute.";
                        return false;
                    }

                    StreamGeometry geometry = TryParseStreamGeometry(geometryCode);
                    if (geometryCode != null)
                    {
                        RenderGeometry(geometry);
                        return true;
                    }
                }
                else
                {
                    exceptionMsg = InvalidShapeFormat;
                }
            }

            return false;
        }

        private static XmlDocument TryLoadXml(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xml);
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
                return null;
            }

            return xmlDoc;
        }

        private static Page TryParsePage(string shape)
        {
            Page page = null;

            try
            {
                page = (Page)XamlReader.Parse(shape);
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
            }

            return page;
        }

        private static StreamGeometry TryParseStreamGeometry(string streamGeometry)
        {
            StreamGeometry geometry = null;

            try
            {
                geometry = (StreamGeometry)Geometry.Parse(streamGeometry);
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
            }

            return geometry;
        }

        private static void RenderGeometry(Geometry geometry)
        {
            Path path = new Path
            {
                Data = geometry,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = geometry.Bounds.Right + strokeThickness,
                Height = geometry.Bounds.Bottom + strokeThickness,
                Stroke = strokeBrush,
                Fill = fillBrush,
                StrokeThickness = strokeThickness
            };

            Canvas canvas = new Canvas
            {
                Width = shapeSize.Width,
                Height = shapeSize.Height,
                Margin = new Thickness(shapeOffset.X, shapeOffset.Y, 0, 0),
                Background = Brushes.Transparent
            };

            canvas.Children.Add(path);

            canvas.Measure(new Size(canvas.Width, canvas.Height));
            canvas.Arrange(new Rect(new Size(canvas.Width, canvas.Height)));

            CreateBitmap(canvas, (int)canvas.Width, (int)canvas.Height);
        }

        private static void RenderPage(Page page)
        {
            page.Width = shapeSize.Width;
            page.Height = shapeSize.Height;
            page.Margin = new Thickness(shapeOffset.X, shapeOffset.Y, 0, 0);

            page.Measure(new Size(page.Width, page.Height));
            page.Arrange(new Rect(new Size(page.Width, page.Height)));

            CreateBitmap(page, (int)page.Width, (int)page.Height);
        }

        private static void CreateBitmap(Visual visual, int width, int height)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            BmpBitmapEncoder image = new BmpBitmapEncoder();
            image.Frames.Add(BitmapFrame.Create(bitmap));
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                image.Save(ms);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Shape = new System.Drawing.Bitmap(ms);
            }
        }
    }
}
