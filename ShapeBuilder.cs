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
        private static Size canvasSize;
        private static Rect selection;
        private static SolidColorBrush strokeBrush;
        private static SolidColorBrush fillBrush;
        private static double strokeThickness;

        private static readonly Error invalidShapeError = Error.NewShapeError(0, 0, "Shape code is invalid or otherwise unrecognized.");
        private static Error error;

        internal static Error Error => error;
        internal static System.Drawing.Bitmap Shape;

        internal static void SetEnviromentParams(int canvasWidth, int canvasHeight, int selectionX, int selectionY,
            int selectionWidth, int selctionHeight, ColorBgra strokeColor, ColorBgra fillColor, double strokeWidth)
        {
            canvasSize = new Size(canvasWidth, canvasHeight);
            selection = new Rect(selectionX, selectionY, selectionWidth, selctionHeight);
            strokeBrush = new SolidColorBrush(Color.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B));
            fillBrush = new SolidColorBrush(Color.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B));
            strokeThickness = strokeWidth;
        }

        internal static bool RenderShape(string shapeCode)
        {
            Shape?.Dispose();
            Shape = null;
            error = null;

            if (string.IsNullOrWhiteSpace(shapeCode))
            {
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
                XmlNodeList paths = docElement.GetElementsByTagName("Path");
                if (paths.Count > 0)
                {
                    Path path = TryParsePath(paths[0].OuterXml);
                    if (path != null)
                    {
                        RenderPath(path);
                        return true;
                    }
                }
            }
            else if (docElement.Name == "ps:SimpleGeometryShape")
            {
                if (docElement.HasChildNodes)
                {
                    XmlNode firstChild = docElement.FirstChild;
                    while (firstChild.NodeType != XmlNodeType.Element)
                    {
                        firstChild = firstChild.NextSibling;
                        if (firstChild == null)
                        {
                            error = invalidShapeError;
                            return false;
                        }
                    }

                    string outerXml = firstChild.OuterXml;
                    int xmlnsStartIndex = outerXml.IndexOf(" xmlns=");
                    int xmlnsEndIndex = outerXml.IndexOf(">");

                    if (xmlnsStartIndex == -1 || xmlnsEndIndex == -1)
                    {
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
                        return false;
                    }

                    StreamGeometry geometry = TryParseStreamGeometry(geometryCode);
                    if (geometry != null)
                    {
                        RenderGeometry(geometry);
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool TryParseShapeCode(string shapeCode)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(shapeCode))
            {
                error = invalidShapeError;
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
                XmlNodeList paths = docElement.GetElementsByTagName("Path");
                if (paths.Count > 0)
                {
                    Path path = TryParsePath(paths[0].OuterXml);
                    if (path != null)
                    {
                        //RenderPath(path);
                        return true;
                    }
                }
                else
                {
                    error = Error.NewShapeError(0, 0, "Can not find the Path tag.");
                }
            }
            else if (docElement.Name == "ps:SimpleGeometryShape")
            {
                if (docElement.HasChildNodes)
                {
                    XmlNode firstChild = docElement.FirstChild;
                    while (firstChild.NodeType != XmlNodeType.Element)
                    {
                        firstChild = firstChild.NextSibling;
                        if (firstChild == null)
                        {
                            error = invalidShapeError;
                            return false;
                        }
                    }

                    string outerXml = firstChild.OuterXml;
                    int xmlnsStartIndex = outerXml.IndexOf(" xmlns=");
                    int xmlnsEndIndex = outerXml.IndexOf(">");

                    if (xmlnsStartIndex == -1 || xmlnsEndIndex == -1)
                    {
                        error = invalidShapeError;
                        return false;
                    }

                    string pageCode = "<Page xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
                        + outerXml.Substring(0, xmlnsStartIndex) + outerXml.Substring(xmlnsEndIndex)
                        + "</Page>";

                    Page page = TryParsePage(pageCode);
                    if (page?.Content is Geometry geometry)
                    {
                        //RenderGeometry(geometry);
                        return true;
                    }
                }
                else if (docElement.HasAttributes && docElement.HasAttribute("Geometry"))
                {
                    string geometryCode = docElement.Attributes.GetNamedItem("Geometry").InnerText;
                    if (string.IsNullOrWhiteSpace(geometryCode))
                    {
                        error = Error.NewShapeError(0, 0, "The Geometry attribute is empty.");
                        return false;
                    }

                    StreamGeometry geometry = TryParseStreamGeometry(geometryCode);
                    if (geometry != null)
                    {
                        //RenderGeometry(geometry);
                        return true;
                    }
                }
                else
                {
                    error = Error.NewShapeError(0, 0, "Can not find the Geometry attribute.");
                }
            }
            else
            {
                error = invalidShapeError;
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
            catch (XmlException ex)
            {
                error = Error.NewShapeError(ex.LineNumber, ex.LinePosition, ex.Message);
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
            catch (XamlParseException ex)
            {
                error = Error.NewShapeError(ex.LineNumber, ex.LinePosition, ex.Message);
            }

            return page;
        }

        private static Path TryParsePath(string shape)
        {
            Path path = null;

            try
            {
                path = (Path)XamlReader.Parse(shape);
            }
            catch (XamlParseException ex)
            {
                error = Error.NewShapeError(ex.LineNumber, ex.LinePosition, ex.Message);
            }

            return path;
        }

        private static StreamGeometry TryParseStreamGeometry(string streamGeometry)
        {
            StreamGeometry geometry = null;

            try
            {
                geometry = (StreamGeometry)Geometry.Parse(streamGeometry);
            }
            catch (FormatException ex)
            {
                error = Error.NewShapeError(0, 0, ex.Message);
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
                Stroke = strokeBrush,
                Fill = fillBrush,
                StrokeThickness = strokeThickness
            };

            RenderPath(path);
        }

        private static void RenderPath(Path path)
        {
            const int padding = 5;
            path.Stretch = Stretch.Uniform;
            path.Width = selection.Width - padding * 2;
            path.Height = selection.Height - padding * 2;
            path.Margin = new Thickness(padding);

            Canvas canvas = new Canvas
            {
                Width = canvasSize.Width,
                Height = canvasSize.Height,
                Margin = new Thickness(selection.X, selection.Y, 0, 0),
                Background = Brushes.Transparent
            };

            canvas.Children.Add(path);

            canvas.Measure(new Size(canvas.Width, canvas.Height));
            canvas.Arrange(new Rect(new Size(canvas.Width, canvas.Height)));

            CreateBitmap(canvas, (int)canvas.Width, (int)canvas.Height);
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
