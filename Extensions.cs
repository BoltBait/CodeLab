/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2017-2018 Jason Wendt. All Rights Reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class Extensions
    {
        internal static string InsertLineBreaks(this string original, int maxCharWidth)
        {
            if (original.Length <= maxCharWidth)
            {
                return original;
            }

            List<string> splitOriginal = new List<string>();
            for (int i = 0; i < original.Length; i += maxCharWidth)
            {
                int segmentLength = maxCharWidth;

                if (i + segmentLength >= original.Length)
                {
                    segmentLength -= i + segmentLength - original.Length;
                }
                else
                {
                    while (!char.IsWhiteSpace(original[i + segmentLength]))
                    {
                        segmentLength--;
                    }
                    segmentLength++;
                }

                splitOriginal.Add(original.Substring(i, segmentLength));

                i -= maxCharWidth - segmentLength;
            }

            return string.Join("\r\n", splitOriginal);
        }

        internal static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        internal static bool IsCSharpIndentifier(this string value)
        {
            if (value.Length == 0)
            {
                return false;
            }

            if (char.IsNumber(value[0]))
            {
                return false;
            }

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    return false;
                }
            }

            return true;
        }

        internal static string StripComments(this string str)
        {
            const string blockComments = @"/\*(.*?)\*/";
            const string lineComments = @"//(.*?)\r?\n";

            return Regex.Replace(
                str,
                blockComments + "|" + lineComments,
                match =>
                {
                    if (match.Value.StartsWith("//", StringComparison.Ordinal))
                    {
                        return "\r\n";
                    }
                    if (match.Value.StartsWith("/*", StringComparison.Ordinal))
                    {
                        string newLines = "";
                        for (int i = 0; i < match.Value.CountLines(); i++)
                        {
                            newLines += "\r\n";
                        }
                        return newLines;
                    }
                    return match.Value;
                },
                RegexOptions.Singleline);
        }

        internal static string StripParens(this string str)
        {
            return Regex.Replace(str, @"\((?:\([^()]*\)|[^()])*\)", string.Empty);
        }

        internal static string StripBraces(this string str)
        {
            return Regex.Replace(str, @"\{(?:\{[^{}]*\}|[^{}])*\}", string.Empty);
        }

        internal static string GetInitials(this string str)
        {
            return new string(str.Where(c => char.IsUpper(c)).ToArray());
        }

        internal static string FirstCharToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsLetter(str[0]))
            {
                return str;
            }

            char capped = char.ToUpperInvariant(str[0]);

            if (str.Length == 1)
            {
                return capped.ToString();
            }

            return capped + str.Substring(1);
        }

        internal static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source?.IndexOf(value, comparisonType) >= 0;
        }

        internal static int CountLines(this string str)
        {
            int count = 0;

            foreach (char c in str)
            {
                if (c == '\n')
                {
                    ++count;
                }
            }

            return count;
        }

        internal static string GetDisplayName(this Type type)
        {
            return (type.IsGenericType) ? type.GetGenericName() : type.GetAliasName();
        }

        internal static string GetGenericName(this Type type)
        {
            string typeName = Regex.Replace(type.Name, @"`\d", string.Empty);

            List<string> genericArgs = new List<string>();
            foreach (Type arg in type.GetGenericArguments())
            {
                genericArgs.Add(arg.GetAliasName());
            }

            return $"{typeName}<{genericArgs.Join(", ")}>";
        }

        internal static bool Contains(this Type type, string memberName, bool onlyUserDefined)
        {
            if (type == null)
            {
                return false;
            }

            BindingFlags flags = (onlyUserDefined) ?
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly :
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            return type.GetMember(memberName, flags).Length > 0;
        }

        internal static char ToChar(this int charCode)
        {
            return (char)charCode;
        }

        internal static char ToUpperInvariant(this char c)
        {
            return char.ToUpperInvariant(c);
        }

        internal static char ToLowerInvariant(this char c)
        {
            return char.ToLowerInvariant(c);
        }

        internal static string GetEnumValue(this FieldInfo fieldInfo)
        {
            object value = fieldInfo.GetValue(null);
            Type type = Enum.GetUnderlyingType(fieldInfo.FieldType);
            return Convert.ChangeType(value, type).ToString();
        }

        internal static string GetterSetter(this PropertyInfo property)
        {
            if (property.CanRead && property.CanWrite)
                return " { get; set; }";
            if (property.CanRead)
                return " { get; }";
            if (property.CanWrite)
                return " { set; }";

            return string.Empty;
        }

        internal static string Params(this MethodBase method)
        {
            List<string> methodParams = new List<string>();
            foreach (ParameterInfo param in method.GetParameters())
            {
                methodParams.Add($"{(param.ParameterType.IsByRef ? "ref " : string.Empty)}{param.ParameterType.GetDisplayName()} {param.Name}");
            }

            return methodParams.Join(", ");
        }

        internal static Type GetReturnType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.NestedType:
                    return (Type)member;
            }

            return null;
        }

        internal static string GetObjectType(this Type type)
        {
            if (type.IsEnum)
            {
                return "enum";
            }
            if (type.IsValueType)
            {
                return "struct";
            }
            if (type.IsClass)
            {
                return "class";
            }
            if (type.IsInterface)
            {
                return "interface";
            }

            return "Type";
        }

        private static string GetAliasName(this Type type)
        {
            string typeName = type.Name;
            if (typeName.EndsWith("&", StringComparison.Ordinal))
            {
                typeName = typeName.TrimEnd('&');
            }

            return typeAliases.ContainsKey(typeName) ? typeAliases[typeName] : typeName;
        }

        private static readonly Dictionary<string, string> typeAliases;

        static Extensions()
        {
            typeAliases = new Dictionary<string, string>
            {
                { "Byte", "byte" },
                { "SByte", "sbyte" },
                { "Int16", "short" },
                { "UInt16", "ushort" },
                { "Int32", "int" },
                { "UInt32", "uint" },
                { "Int64", "long" },
                { "UInt64", "ulong" },
                { "Single", "float" },
                { "Double", "double" },
                { "Decimal", "decimal" },
                { "Boolean", "bool" },
                { "Char", "char" },
                { "String", "string" },
                { "Object", "object" },
                { "Byte[]", "byte[]" },
                { "SByte[]", "sbyte[]" },
                { "Int16[]", "short[]" },
                { "UInt16[]", "ushort[]" },
                { "Int32[]", "int[]" },
                { "UInt32[]", "uint[]" },
                { "Int64[]", "long[]" },
                { "UInt64[]", "ulong[]" },
                { "Single[]", "float[]" },
                { "Double[]", "double[]" },
                { "Decimal[]", "decimal[]" },
                { "Boolean[]", "bool[]" },
                { "Char[]", "char[]" },
                { "String[]", "string[]" },
                { "Object[]", "object[]" },
                { "Void", "void" }
            };
        }
    }
}
