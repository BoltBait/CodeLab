using PaintDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PdnCodeLab
{
    [Flags]
    internal enum DocCommentOptions
    {
        None = 0,
        Definitions = 1,
        ToolTips = 2,
        BCL = 4,
        Enabled = 8,
        Default = Enabled | Definitions | ToolTips
    }

    internal static class DocComment
    {
        private static DocCommentOptions options = Settings.DocCommentOptions;
        private static Dictionary<string, XElement> docComments = IngestDocXML();

        private static Dictionary<string, XElement> IngestDocXML()
        {
            if (options <= DocCommentOptions.Enabled)
            {
                return new Dictionary<string, XElement>();
            }

            IEnumerable<string> pdnXml = Directory.EnumerateFiles(Application.StartupPath, "*.xml", SearchOption.TopDirectoryOnly);

            IEnumerable<string> bclXml = (options.HasFlag(DocCommentOptions.BCL) && TryGetSdkDirectory(out string sdkDir))
                ? bclXml = Directory.EnumerateFiles(sdkDir, "*.xml", SearchOption.TopDirectoryOnly)
                : Array.Empty<string>();

            return pdnXml
                .Concat(bclXml)
                .AsParallel()
                .SelectMany(xmlPath =>
                {
                    XDocument xDoc = XDocument.Load(xmlPath);
                    XElement docElement = xDoc.Root;

                    if (docElement.Name.LocalName != "doc" || !docElement.HasElements)
                    {
                        return Array.Empty<KeyValuePair<string, XElement>>();
                    }

                    XElement members = docElement.Element("members");
                    if (members == null || !members.HasElements)
                    {
                        return Array.Empty<KeyValuePair<string, XElement>>();
                    }

                    return members
                        .Elements("member")
                        .Where(e => e.Elements("summary").Any())
                        .Select(e => new KeyValuePair<string, XElement>(e.Attribute("name").Value, e));
                })
                .DistinctBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static void ReInstantiate()
        {
            DocCommentOptions newOptions = Settings.DocCommentOptions;
            if (newOptions != options)
            {
                options = newOptions;
                docComments = IngestDocXML();
            }
        }

        internal static bool TryGetSdkDirectory(out string sdkDirectory)
        {
            sdkDirectory = null;

            string sdkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"dotnet\packs\Microsoft.NETCore.App.Ref");
            if (!Directory.Exists(sdkPath))
            {
                return false;
            }

            string dotnetVer = Environment.Version.ToString(2);

            string latestDir = Directory.EnumerateDirectories(sdkPath, dotnetVer + ".*", SearchOption.TopDirectoryOnly)
                .OrderByDescending(dir => Directory.GetCreationTime(dir))
                .FirstOrDefault();

            if (latestDir == null)
            {
                return false;
            }

            string sdkDir = Path.Combine(latestDir, @"ref\net" + dotnetVer);
            if (!Directory.Exists(sdkDir))
            {
                return false;
            }

            sdkDirectory = sdkDir;
            return true;
        }

        internal static string GetDocCommentForDef(this MemberInfo memberInfo, string indentSpacing)
        {
            if (options <= DocCommentOptions.Enabled || !options.HasFlag(DocCommentOptions.Definitions))
            {
                return string.Empty;
            }

            string commentKey = BuildCommentKey(memberInfo);
            if (!docComments.TryGetValue(commentKey, out XElement comment))
            {
                return string.Empty;
            }

            string summary = comment.Element("summary").NormalizeCommentElement();
            if (summary.Length == 0)
            {
                return string.Empty;
            }

            string remarks = comment.Element("remarks")?.NormalizeCommentElement() ?? string.Empty;
            string returns = comment.Element("returns")?.NormalizeCommentElement() ?? string.Empty;

            return
                indentSpacing + "//\r\n" +
                indentSpacing + "// Summary:\r\n" +
                indentSpacing + "//     " + summary.Split(Environment.NewLine).Join(Environment.NewLine + indentSpacing + "//     ") + "\r\n" +

                (returns.Length == 0 ? string.Empty :
                indentSpacing + "//\r\n" +
                indentSpacing + "// Returns:\r\n" +
                indentSpacing + "//     " + returns.Split(Environment.NewLine).Join(Environment.NewLine + indentSpacing + "//     ") + "\r\n") +

                (remarks.Length == 0 ? string.Empty :
                indentSpacing + "//\r\n" +
                indentSpacing + "// Remarks:\r\n" +
                indentSpacing + "//     " + remarks.Split(Environment.NewLine).Join(Environment.NewLine + indentSpacing + "//     ") + "\r\n");
        }

        internal static string GetDocCommentForToolTip(this MemberInfo memberInfo)
        {
            if (options <= DocCommentOptions.Enabled || !options.HasFlag(DocCommentOptions.ToolTips))
            {
                return string.Empty;
            }

            string commentKey = BuildCommentKey(memberInfo);
            if (!docComments.TryGetValue(commentKey, out XElement comment))
            {
                return string.Empty;
            }

            string summary = comment.Element("summary").NormalizeCommentElement();
            if (summary.Length == 0)
            {
                return string.Empty;
            }

            string remarks = comment.Element("remarks")?.NormalizeCommentElement() ?? string.Empty;
            string returns = comment.Element("returns")?.NormalizeCommentElement() ?? string.Empty;

            return "\r\n\r\n" +
                summary +
                (remarks.Length > 0 ? "\r\n\r\n" + remarks : string.Empty) +
                (returns.Length > 0 ? "\r\n\r\nReturns: " + returns : string.Empty);
        }

        internal static string GetDocCommentForIntelliBox(this MemberInfo memberInfo)
        {
            if (options <= DocCommentOptions.Enabled || !options.HasFlag(DocCommentOptions.ToolTips))
            {
                return string.Empty;
            }

            string commentKey = BuildCommentKey(memberInfo);
            if (!docComments.TryGetValue(commentKey, out XElement comment))
            {
                return string.Empty;
            }

            string summary = comment.Element("summary").NormalizeCommentElement();
            if (summary.Length == 0)
            {
                return string.Empty;
            }

            return "\r\n\r\n" + summary;
        }

        private static string BuildCommentKey(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
                    return "P:" + memberInfo.DeclaringType.FullName.Replace('+', '.') + "." + memberInfo.Name + propertyInfo.GetIndexParameters().BuildParamString();
                case MemberTypes.Method:
                    MethodInfo methodInfo = (MethodInfo)memberInfo;

                    if (methodInfo.DeclaringType.IsConstructedGenericType)
                    {
                        MethodInfo replacementMethod = methodInfo
                            .DeclaringType
                            .GetGenericTypeDefinition()
                            .GetMethods()
                            .FirstOrDefault(m => m.Name == methodInfo.Name && m.GetParameters().Length == methodInfo.GetParameters().Length);

                        if (replacementMethod != null)
                        {
                            methodInfo = replacementMethod;
                        }
                    }

                    string generic = methodInfo.IsGenericMethod ? "``" + methodInfo.GetGenericArguments().Length : string.Empty;
                    return "M:" + memberInfo.DeclaringType.GetFullName() + "." + memberInfo.Name + generic + methodInfo.GetParameters().BuildParamString();
                case MemberTypes.Field:
                    return "F:" + memberInfo.DeclaringType.FullName.Replace('+', '.') + "." + memberInfo.Name;
                case MemberTypes.Event:
                    return "E:" + memberInfo.DeclaringType.FullName.Replace('+', '.') + "." + memberInfo.Name;
                case MemberTypes.TypeInfo:
                    Type type = (Type)memberInfo;
                    return "T:" + type.FullName;
                case MemberTypes.NestedType:
                    return "T:" + memberInfo.DeclaringType.FullName.Replace('+', '.') + "." + memberInfo.Name;
                case MemberTypes.Constructor:
                    ConstructorInfo ctorInfo = (ConstructorInfo)memberInfo;
                    return "M:" + memberInfo.DeclaringType.FullName.Replace('+', '.') + ".#ctor" + ctorInfo.GetParameters().BuildParamString();
                case MemberTypes.Custom:
                case MemberTypes.All:
                    throw new NotImplementedException();
            }

            return null;
        }

        private static string BuildParamString(this IReadOnlyCollection<ParameterInfo> parameters)
        {
            if (parameters.Count == 0)
            {
                return string.Empty;
            }

            string paramString = parameters
                .Select(pi =>
                {
                    string typeName = pi.ParameterType.IsGenericType
                        ? pi.ParameterType.GetGenericFullName()
                        : pi.ParameterType.IsGenericParameter
                            ? "`0"
                            : pi.ParameterType.FullName ?? pi.ParameterType.Name;

                    if (pi.ParameterType.IsNested)
                    {
                        typeName = typeName.Replace('+', '.');
                    }

                    if (pi.ParameterType.IsByRef)
                    {
                        typeName = typeName.Replace('&', '@');
                    }

                    return typeName;
                })
                .Join(",");

            return "(" + paramString + ")";
        }

        private static string GetGenericFullName(this Type type)
        {
            string typeName = Regex.Replace(type.Name, @"`\d", string.Empty);

            List<string> argList = new List<string>();
            foreach (Type argType in type.GetGenericArguments())
            {
                string arg = string.Empty;

                if (argType.IsGenericParameter)
                {
                    GenericParameterAttributes attributes = argType.GenericParameterAttributes;
                    if (attributes == GenericParameterAttributes.Contravariant)
                    {
                        arg += "in ";
                    }
                    else if (attributes == GenericParameterAttributes.Covariant)
                    {
                        arg += "out ";
                    }
                }

                arg += argType.FullName ?? "``0";

                argList.Add(arg);
            }

            return $"{type.Namespace}.{typeName}{{{argList.Join(',')}}}";
        }

        private static string GetFullName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName.Replace('+', '.');
            }

            if (type.IsConstructedGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return $"{type.Namespace}.{type.Name}";
        }

        private static string NormalizeCommentElement(this XElement commentElement)
        {
            if (commentElement.HasElements)
            {
                foreach (XElement inheritdocElement in commentElement.Elements("inheritdoc"))
                {
                    if (inheritdocElement.HasAttributes)
                    {
                        string cref = inheritdocElement.Attribute("cref")?.Value;
                        if (cref != null)
                        {
                            inheritdocElement.Value = docComments.TryGetValue(cref, out XElement summary)
                                ? summary.Element(commentElement.Name)?.NormalizeCommentElement() ?? string.Empty
                                : string.Empty;
                        }
                    }
                }

                foreach (XElement seeElement in commentElement.Elements("see"))
                {
                    if (seeElement.HasAttributes)
                    {
                        string cref = seeElement.Attribute("cref")?.Value;
                        if (cref != null)
                        {
                            string crefNormalized = cref.StripParens();
                            int dotIndex = crefNormalized.LastIndexOf('.');
                            if (dotIndex != -1)
                            {
                                if (cref[0] != 'T')
                                {
                                    int penultimateDotIndex = crefNormalized.LastIndexOf('.', dotIndex - 1);
                                    if (penultimateDotIndex != -1)
                                    {
                                        dotIndex = penultimateDotIndex;
                                    }
                                }

                                dotIndex++;
                                string crefShortened = cref[dotIndex..];
                                seeElement.Value = Regex.Replace(crefShortened, @"``\d+|{``\d+}|`\d+", "<T>");
                            }
                        }
                        else
                        {
                            XAttribute attribute = seeElement.Attributes().FirstOrDefault();
                            if (attribute != null)
                            {
                                seeElement.Value = attribute.Value;
                            }
                        }
                    }
                }

                foreach (XElement anchorElement in commentElement.Elements("a"))
                {
                    if (anchorElement.Value[0] != ' ')
                    {
                        anchorElement.Value = " " + anchorElement.Value;
                    }
                }

                foreach (XElement breakElement in commentElement.Elements("br"))
                {
                    breakElement.Value = char.MinValue.ToString();
                }
            }

            return commentElement.Value
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Join(" ")
                .Split(char.MinValue, StringSplitOptions.TrimEntries)
                .Join(Environment.NewLine);
        }
    }
}
