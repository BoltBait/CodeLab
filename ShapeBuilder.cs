﻿using System;
using System.Linq;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace PdnCodeLab
{
    internal static class ShapeBuilder
    {
        private static Error error;

        internal static Error Error => error;

        internal static bool TryParseShapeCode(string shapeCode)
        {
            Geometry geometry = GeometryFromRawString(shapeCode);

            return geometry != null;
        }

        internal static bool TryExtractShapeName(string shapeCode, out string shapeName)
        {
            shapeName = null;

            if (string.IsNullOrWhiteSpace(shapeCode))
            {
                return false;
            }

            XDocument xDoc = TryParseXDocument(shapeCode);
            if (xDoc == null)
            {
                return false;
            }

            XElement docElement = xDoc.Root;
            if (docElement.Name.LocalName != "SimpleGeometryShape" || !docElement.HasAttributes)
            {
                return false;
            }

            XAttribute nameAttribute = docElement.Attribute("DisplayName");
            if (nameAttribute == null || string.IsNullOrWhiteSpace(nameAttribute.Value))
            {
                return false;
            }

            shapeName = nameAttribute.Value;
            return shapeName != null;
        }

        internal static Geometry GeometryFromRawString(string shapeCode)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(shapeCode))
            {
                error = Error.NewShapeError(0, 0, "Shape code is invalid or otherwise unrecognized.");
                return null;
            }

            XDocument xDoc = TryParseXDocument(shapeCode);
            if (xDoc == null)
            {
                return null;
            }

            XElement docElement = xDoc.Root;

            if (docElement.Name.LocalName != "SimpleGeometryShape")
            {
                IXmlLineInfo lineInfo = docElement;
                error = Error.NewShapeError(lineInfo.LineNumber, 0, "The root element must be SimpleGeometryShape.");
                return null;
            }

            if (!docElement.HasAttributes)
            {
                IXmlLineInfo lineInfo = docElement;
                error = Error.NewShapeError(lineInfo.LineNumber, 0, "The SimpleGeometryShape element is missing attributes.");
                return null;
            }

            XAttribute nameAttribute = docElement.Attribute(XName.Get("DisplayName", string.Empty));
            if (nameAttribute == null)
            {
                IXmlLineInfo lineInfo = docElement;
                error = Error.NewShapeError(lineInfo.LineNumber, 0, "The DisplayName attribute is missing.");
            }
            else if (string.IsNullOrWhiteSpace(nameAttribute.Value))
            {
                IXmlLineInfo lineInfo = nameAttribute;
                error = Error.NewShapeError(lineInfo.LineNumber, 0, "The DisplayName attribute is empty.");
            }

            XAttribute geometryAttribute = docElement.Attribute(XName.Get("Geometry", string.Empty));

            if (docElement.HasElements)
            {
                if (geometryAttribute != null)
                {
                    IXmlLineInfo lineInfo = geometryAttribute;
                    error = Error.NewShapeError(lineInfo.LineNumber, 0, "Can not contain both Geometry attribute and child elements.");
                    return null;
                }

                XElement firstElement = docElement.Elements().First();

                if (firstElement.ElementsAfterSelf().Any())
                {
                    IXmlLineInfo lineInfo = firstElement.ElementsAfterSelf().First();
                    error = Error.NewShapeError(lineInfo.LineNumber, 0, "There can only be one child element of SimpleGeometryShape.");
                    return null;
                }

                if (!Intelli.XamlAutoCompleteTypes.TryGetValue(firstElement.Name.LocalName, out Type type) ||
                    !type.IsSubclassOf(typeof(Geometry)))
                {
                    IXmlLineInfo lineInfo = firstElement;
                    error = Error.NewShapeError(lineInfo.LineNumber, 0, "The child element of SimpleGeometryShape must be a Geometry-derived class.");
                    return null;
                }

                string xElementText = firstElement.ToString();
                int xmlnsStartIndex = xElementText.IndexOf(" xmlns=", StringComparison.Ordinal);
                int xmlnsEndIndex = xElementText.IndexOf(">", StringComparison.Ordinal);

                if (firstElement.IsEmpty)
                {
                    xmlnsEndIndex--;
                }

                if (xmlnsStartIndex < 0 || xmlnsEndIndex < 0 || xmlnsEndIndex < xmlnsStartIndex)
                {
                    error = Error.NewShapeError(0, 0, "Shape code is invalid or otherwise unrecognized.");
                    return null;
                }

                const string xamlNs = " xmlns =\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"";

                IXmlLineInfo offsetPos = firstElement;
                string whiteSpace = new string('\n', offsetPos.LineNumber - 1) + new string(' ', offsetPos.LinePosition - 2);

                string geometryText = xElementText
                    .Remove(xmlnsStartIndex, xmlnsEndIndex - xmlnsStartIndex)
                    .Insert(xmlnsStartIndex, xamlNs)
                    .Insert(0, whiteSpace);

                return TryParseGeometry(geometryText);
            }
            else
            {
                if (geometryAttribute == null)
                {
                    error = Error.NewShapeError(0, 0, "Can not find the Geometry attribute.");
                    return null;
                }

                string geometryText = geometryAttribute.Value;
                if (string.IsNullOrWhiteSpace(geometryText))
                {
                    IXmlLineInfo lineInfo = geometryAttribute;
                    error = Error.NewShapeError(lineInfo.LineNumber, 0, "The Geometry attribute is empty.");
                    return null;
                }

                return TryParseStreamGeometry(geometryText);
            }
        }

        private static XDocument TryParseXDocument(string xDocumentText)
        {
            XDocument xDocument = null;

            try
            {
                xDocument = XDocument.Parse(xDocumentText, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            }
            catch (XmlException ex)
            {
                error = Error.NewShapeError(ex.LineNumber, ex.LinePosition, ex.Message);
                return null;
            }

            return xDocument;
        }

        private static Geometry TryParseGeometry(string geometryText)
        {
            Geometry geometry = null;

            try
            {
                geometry = (Geometry)XamlReader.Parse(geometryText);
            }
            catch (XamlParseException ex)
            {
                error = Error.NewShapeError(ex.LineNumber, ex.LinePosition, ex.Message);
            }

            return geometry;
        }

        private static StreamGeometry TryParseStreamGeometry(string streamGeometryText)
        {
            StreamGeometry streamGeometry = null;

            try
            {
                streamGeometry = (StreamGeometry)Geometry.Parse(streamGeometryText);
            }
            catch (FormatException ex)
            {
                error = Error.NewShapeError(0, 0, ex.Message);
            }

            return streamGeometry;
        }
    }
}
