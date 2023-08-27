using Microsoft.CodeAnalysis;
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
    internal static class DocComment
    {
        private static readonly Dictionary<string, string> docComments = IngestDocXML();

        private static Dictionary<string, string> IngestDocXML()
        {
            IEnumerable<string> pdnXml = Directory.EnumerateFiles(Application.StartupPath, "*.xml", SearchOption.TopDirectoryOnly);
            IEnumerable<string> bclXml = Array.Empty<string>();

            string sdkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"dotnet\packs\Microsoft.NETCore.App.Ref");
            if (Directory.Exists(sdkPath))
            {
                string dotnetVer = Environment.Version.ToString(2);

                string latestDir = Directory.EnumerateDirectories(sdkPath, dotnetVer + ".*", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(dir => Directory.GetCreationTime(dir))
                    .FirstOrDefault();

                if (latestDir != null)
                {
                    string xmlDir = Path.Combine(latestDir, @"ref\net" + dotnetVer);
                    if (Directory.Exists(xmlDir))
                    {
                        bclXml = Directory.EnumerateFiles(xmlDir, "*.xml", SearchOption.TopDirectoryOnly);
                    }
                }
            }

            return pdnXml
                .Concat(bclXml)
                .AsParallel()
                .SelectMany(xmlPath =>
                {
                    XDocument xDoc = XDocument.Load(xmlPath);
                    XElement docElement = xDoc.Root;

                    if (docElement.Name.LocalName != "doc" || !docElement.HasElements)
                    {
                        return Array.Empty<KeyValuePair<string, string>>();
                    }

                    XElement members = docElement.Element("members");
                    if (members == null || !members.HasElements)
                    {
                        return Array.Empty<KeyValuePair<string, string>>();
                    }

                    return members
                        .Elements("member")
                        .Where(e => e.Elements("summary").Any())
                        .Select(e => new KeyValuePair<string, string>(e.Attribute("name").Value, e.Element("summary").NormalizeSummaryTag()));
                })
                .DistinctBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static string GetDocSummaryForDef(this MemberInfo memberInfo, string indentSpacing)
        {
            string commentKey = BuildCommentKey(memberInfo);

            return docComments.TryGetValue(commentKey, out string summary)
                ? indentSpacing + "//\r\n" +
                    indentSpacing + "// Summary:\r\n" +
                    indentSpacing + "//     " + summary + "\r\n"
                : string.Empty;
        }

        internal static string GetDocCommentForToolTip(this MemberInfo memberInfo)
        {
            string commentKey = BuildCommentKey(memberInfo);

            return docComments.TryGetValue(commentKey, out string summary)
                ? "\r\n\r\n" + summary
                : string.Empty;
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

        private static string NormalizeSummaryTag(this XElement summaryElement)
        {
            if (summaryElement.HasElements)
            {
                foreach (XElement subElement in summaryElement.Elements("see"))
                {
                    if (subElement.HasAttributes)
                    {
                        string cref = subElement.Attribute("cref")?.Value;
                        if (cref != null)
                        {
                            int dotIndex = cref.StripParens().LastIndexOf('.');
                            if (dotIndex != -1)
                            {
                                dotIndex++;
                                subElement.Value = cref[dotIndex..];
                            }
                        }
                        else
                        {
                            XAttribute attribute = subElement.Attributes().FirstOrDefault();
                            if (attribute != null)
                            {
                                subElement.Value = attribute.Value;
                            }
                        }
                    }
                }
            }

            return summaryElement.Value
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Join(' ');
        }
    }
}
