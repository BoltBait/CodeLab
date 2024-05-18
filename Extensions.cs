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

using PaintDotNet;
using PluralizeService.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PdnCodeLab
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

                    if (segmentLength == 0)
                    {
                        segmentLength = maxCharWidth;
                    }
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

        internal static bool IsCSharpIdentifier(this string value)
        {
            if (value.Length == 0 || char.IsNumber(value[0]))
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

        internal static bool IsWebAddress(this string str)
        {
            return str.Length > 0 && (str.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || str.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
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
                    else if (match.Value.StartsWith("/*", StringComparison.Ordinal))
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

        internal static string StripAngleBrackets(this string str)
        {
            return Regex.Replace(str, @"\<(?:\<[^<>]*\>|[^<>])*\>", string.Empty);
        }

        internal static string StripSquareBrackets(this string str)
        {
            return Regex.Replace(str, @"\[(?:\[[^[\]]*\]|[^[\]])*\]", string.Empty);
        }

        internal static string GetInitials(this string str)
        {
            return new string(str.Where(c => char.IsUpper(c)).ToArray());
        }

        internal static string SplitCamel(this string str)
        {
            return Regex.Replace(str, "(\\B[A-Z])", " $1");
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

        internal static string FirstCharToLower(this string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsLetter(str[0]))
            {
                return str;
            }

            char unCapped = char.ToLowerInvariant(str[0]);

            if (str.Length == 1)
            {
                return unCapped.ToString();
            }

            return unCapped + str.Substring(1);
        }

        internal static string MakePlural(this string str)
        {
            return PluralizationProvider.Pluralize(str);
        }

        internal static int CountLines(this string str)
        {
            return str.Count(c => c == '\n');
        }

        internal static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (T i in ie)
            {
                action(i);
            }
        }

        internal static string GetDisplayName(this Type type)
        {
            if (type.HasElementType)
            {
                Type elementType = type.GetElementType();
                if (elementType.IsGenericType)
                {
                    return elementType.GetGenericName() + (type.IsArray ? "[]" : string.Empty);
                }
            }

            return (type.IsGenericType) ? type.GetGenericName() : type.GetAliasName();
        }

        internal static string GetDisplayNameWithExclusion(this Type type, Type typeExclusion)
        {
            return (type.IsGenericType) ? type.GetGenericName() : (type == typeExclusion) ? type.Name : type.GetAliasName();
        }

        internal static string GetGenericName(this Type type)
        {
            if (type.IsValueType && type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type).GetDisplayName();
            }

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

                arg += argType.GetDisplayName();

                argList.Add(arg);
            }

            return $"{typeName}<{argList.Join(", ")}>";
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

        private static string ToLiteral(this char c)
        {
            switch (c)
            {
                case '\'': return @"\'";
                case '\"': return "\\\"";
                case '\\': return @"\\";
                case '\0': return @"\0";
                case '\a': return @"\a";
                case '\b': return @"\b";
                case '\f': return @"\f";
                case '\n': return @"\n";
                case '\r': return @"\r";
                case '\t': return @"\t";
                case '\v': return @"\v";
                default:
                    if (char.GetUnicodeCategory(c) != UnicodeCategory.Control && !c.Equals('\uffff'))
                    {
                        return c.ToString();
                    }
                    else
                    {
                        return @"\u" + ((ushort)c).ToString("x4");
                    }
            }
        }

        internal static bool IsBrace(this char c, bool openBrace)
        {
            if (openBrace)
            {
                switch (c)
                {
                    case '(':
                    case '[':
                    case '{':
                    //case '<':
                        return true;
                }
            }
            else
            {
                switch (c)
                {
                    case ')':
                    case ']':
                    case '}':
                    //case '>':
                        return true;
                }
            }
            return false;
        }

        private static string ObjectToString(this object obj)
        {
            if (obj is null)
            {
                return "null";
            }

            if (obj is string)
            {
                return "\"" + obj.ToString() + "\"";
            }

            if (obj is char c)
            {
                return "'" + c.ToLiteral() + "'";
            }

            if (obj is Enum)
            {
                Type enumType = obj.GetType();
                return enumType.Name + "." + obj.ToString();
            }

            return obj.ToString();
        }

        internal static MethodInfo MakeGenericMethod(this MethodInfo method, string args)
        {
            Type[] types = StringToTypeArray(args);
            if (types.Length == 0 || types.Length != method.GetGenericArguments().Length)
            {
                return method;
            }

            try
            {
                return method.MakeGenericMethod(types);
            }
            catch
            {
                return method;
            }
        }

        internal static string GetEnumValue(this FieldInfo field)
        {
            object value = field.GetValue(null);
            Type type = Enum.GetUnderlyingType(field.FieldType);
            return Convert.ChangeType(value, type).ToString();
        }

        internal static string GetConstValue(this FieldInfo field)
        {
            return field.GetValue(null).ObjectToString();
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

        internal static bool IsIndexer(this PropertyInfo property)
        {
            return property.GetIndexParameters().Length > 0;
        }

        internal static string Params(this MethodBase method, bool skipExtensionParam = true)
        {
            List<string> methodParams = new List<string>();
            bool isExtension = method.IsOrHasExtension();

            foreach (ParameterInfo param in method.GetParameters())
            {
                if (skipExtensionParam && param.Position == 0 && isExtension)
                {
                    continue;
                }

                methodParams.Add(param.BuildParamString());
            }

            return (isExtension && !skipExtensionParam)
                ? "this " + methodParams.Join(", ")
                : methodParams.Join(", ");
        }

        internal static string BuildParamString(this ParameterInfo parameter)
        {
            string modifier = parameter.IsOut ? "out " : parameter.ParameterType.IsByRef ? "ref " : parameter.IsDefined(typeof(ParamArrayAttribute), false) ? "params " : string.Empty;
            string defaultValue = parameter.HasDefaultValue ? parameter.GetDefaultValue() : string.Empty;
            string nullable = parameter.IsNullable() ? "?" : string.Empty;
            return $"{modifier}{parameter.ParameterType.GetDisplayName()}{nullable} {parameter.Name}{defaultValue}";
        }

        internal static bool IsOrHasExtension(this MemberInfo member)
        {
            return member.IsDefined(typeof(ExtensionAttribute), false);
        }

        private static string GetDefaultValue(this ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsValueType && parameter.RawDefaultValue is null)
            {
                return " = default";
            }

            return " = " + parameter.DefaultValue.ObjectToString();
        }

        internal static Type ExtendingType(this MethodInfo method)
        {
            return method.GetParameters()[0].ParameterType;
        }

        internal static MethodInfo Extends(this MethodInfo method, Type type)
        {
            Type extType = method.ExtendingType();
            if (extType.IsAssignableFrom(type))
            {
                return method;
            }

            if (extType.IsAssignableFromAsIEnumerable(type))
            {
                if (method.GetGenericArguments().Length == 1)
                {
                    if (type.IsConstructedGenericType)
                    {
                        return method.MakeGenericMethod(type.GenericTypeArguments[0]);
                    }

                    if (type.IsArray)
                    {
                        return method.MakeGenericMethod(type.GetElementType());
                    }
                }

                return method;
            }

            return null;
        }

        private static bool IsAssignableFromAsIEnumerable(this Type type1, Type type)
        {
            if (!type1.IsInterface || !type1.Name.Equals("IEnumerable`1", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Type iEnumType = type.GetInterface("IEnumerable`1");
            if (iEnumType == null)
            {
                if (type.IsInterface && type.IsConstructedGenericType && type.Name.Equals("IEnumerable`1", StringComparison.OrdinalIgnoreCase))
                {
                    iEnumType = type;
                }

                if (iEnumType == null)
                {
                    return false;
                }
            }

            Type[] args = type1.GenericTypeArguments;
            if (args.Length != 1 || !args[0].IsGenericParameter)
            {
                return false;
            }

            Type innerType = iEnumType.GenericTypeArguments[0];

            if (args[0].IsConstrainedToClass() && !innerType.IsClass)
            {
                return false;
            }

            Type[] constraints = args[0].GetGenericParameterConstraints();
            if (!constraints.All(t => t.IsAssignableFrom(innerType)))
            {
                return false;
            }

            Type genericType = type1.GetGenericTypeDefinition().MakeGenericType(innerType);

            return genericType.IsAssignableFrom(type);
        }

        private static bool IsConstrainedToClass(this Type type)
        {
            return (type.GenericParameterAttributes &
                GenericParameterAttributes.SpecialConstraintMask &
                GenericParameterAttributes.ReferenceTypeConstraint) != 0;
        }

        internal static Type GetReturnType(this MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                MemberTypes.Method => ((MethodInfo)member).ReturnType,
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Event => ((EventInfo)member).EventHandlerType,
                MemberTypes.NestedType => (Type)member,
                _ => null,
            };
        }

        internal static bool IsObsolete(this MemberInfo member)
        {
            return member.IsDefined(typeof(ObsoleteAttribute), false);
        }

        internal static bool IsCompilerGenerated(this MemberInfo member)
        {
            return member.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }

        internal static bool IsVisibleToReflectedType(this MethodInfo method)
        {
            return method.IsPublic || method.IsFamily || method.DeclaringType == method.ReflectedType;
        }

        internal static bool IsVisibleToReflectedType(this PropertyInfo property)
        {
            return property.GetMethod.IsPublic || property.GetMethod.IsFamily || property.DeclaringType == property.ReflectedType;
        }

        internal static bool IsDelegate(this Type type)
        {
            return typeof(Delegate).IsAssignableFrom(type);
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
            if (type.IsDelegate())
            {
                return "delegate";
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
            string typeName = type.IsByRef ? type.Name.TrimEnd('&') : type.Name;
            return Intelli.TypeAliases.TryGetValue(typeName, out string alias) ? alias : typeName;
        }

        internal static Type MakeGenericType(this Type type, string args)
        {
            Type[] types = StringToTypeArray(args);
            if (types.Length == 0 || types.Length != type.GetGenericArguments().Length)
            {
                return type;
            }

            try
            {
                return type.MakeGenericType(types);
            }
            catch
            {
                return type;
            }
        }

        internal static IEnumerable<string> GetConstraints(this IEnumerable<Type> args)
        {
            List<string> constraints = new List<string>();
            foreach (Type arg in args)
            {
                Type[] argConstraints = arg.GetGenericParameterConstraints();
                bool mustBeClass = arg.IsConstrainedToClass();

                if (argConstraints.Length > 0 || mustBeClass)
                {
                    IEnumerable<string> types = argConstraints.Select(t => t.GetDisplayName());
                    if (mustBeClass)
                    {
                        types = types.Prepend("class");
                    }

                    constraints.Add($"where {arg.GetDisplayName()} : {types.Join(", ")}");
                }
            }

            return constraints;
        }

        internal static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            return type.GetField(name)?.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
        }

        private static Type[] StringToTypeArray(string types)
        {
            if (types.Length == 0)
            {
                return Array.Empty<Type>();
            }

            string[] argArray = types.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (argArray.Length == 0)
            {
                return Array.Empty<Type>();
            }

            List<Type> argTypes = new List<Type>();
            foreach (string arg in argArray)
            {
                if (Intelli.AllTypes.TryGetValue(arg, out Type t))
                {
                    argTypes.Add(t);
                }
                else
                {
                    return Array.Empty<Type>();
                }
            }

            return argTypes.ToArray();
        }

        internal static int FindStringExact(this ListBox listBox, string s, bool preferCaseMatch)
        {
            if (preferCaseMatch)
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    if (listBox.Items[i].ToString().Equals(s, StringComparison.Ordinal))
                    {
                        return i;
                    }
                }
            }

            return listBox.FindStringExact(s);
        }

        internal static int FindString(this ListBox listBox, string s, bool preferCaseMatch)
        {
            if (preferCaseMatch)
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                {
                    if (listBox.Items[i].ToString().StartsWith(s, StringComparison.Ordinal))
                    {
                        return i;
                    }
                }
            }

            return listBox.FindString(s);
        }

        internal static void InvalidateSelectedIndex(this ListBox listBox)
        {
            int selectedIndex = listBox.SelectedIndex;
            if (selectedIndex > -1)
            {
                listBox.Invalidate(listBox.GetItemRectangle(selectedIndex));
            }
        }

        internal static bool IsNullable(this MethodInfo method)
        {
            return nullabilityInfo
                .Create(method.ReturnParameter)
                .ReadState == NullabilityState.Nullable;
        }

        internal static bool IsNullable(this PropertyInfo property)
        {
            return nullabilityInfo
                .Create(property)
                .ReadState == NullabilityState.Nullable;
        }

        internal static bool IsNullable(this FieldInfo field)
        {
            return nullabilityInfo
                .Create(field)
                .ReadState == NullabilityState.Nullable;
        }

        internal static bool IsNullable(this ParameterInfo parameter)
        {
            return nullabilityInfo
                .Create(parameter)
                .ReadState == NullabilityState.Nullable;
        }

        private static readonly NullabilityInfoContext nullabilityInfo = new NullabilityInfoContext();
    }
}
