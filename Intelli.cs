/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2017-2018 Jason Wendt. All Rights Reserved.
// Portions Copyright ©2016 BoltBait. All Rights Reserved.
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
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PdnCodeLab
{
    internal static class Intelli
    {
        internal static Dictionary<string, Type> Variables { get; }
        internal static Dictionary<string, int> VarPos { get; }
        internal static Dictionary<string, Type> Parameters { get; }
        internal static Dictionary<string, Type> UserDefinedTypes { get; }
        internal static Dictionary<string, Type> AutoCompleteTypes { get; private set; }
        internal static Dictionary<string, Type> XamlAutoCompleteTypes { get; }
        internal static Dictionary<string, Type> AllTypes { get; private set; }
        internal static Dictionary<string, string> Snippets { get; }
        internal static Dictionary<string, string> TypeAliases { get; }
        internal static IEnumerable<string> Keywords { get; }
        internal static IEnumerable<string> PdnAssemblyNames { get; private set; }
        internal static Type UserScript { get; set; }
        internal static string ClassList { get; private set; }
        internal static string EnumList { get; private set; }
        internal static string StructList { get; private set; }
        internal static string InterfaceList { get; private set; }

        internal const string UserScriptFullName = "PdnCodeLab.UserScript";
        private static IEnumerable<MethodInfo> extMethods;

        private static readonly ImmutableArray<Assembly> sdkAssemblies;
        private static readonly ImmutableArray<Assembly> allPdnAssemblies;
        private static readonly ImmutableArray<KeyValuePair<string, Type>> aliasTypes;

        internal static IEnumerable<MethodInfo> GetExtensionMethod(this Type extendedType, string methodName)
        {
            return extMethods
                .Where(method => method.Name == methodName)
                .Select(method => method.Extends(extendedType))
                .Where(method => method != null);
        }

        internal static IEnumerable<MethodInfo> GetExtensionMethods(this Type extendedType)
        {
            return extMethods
                .Select(method => method.Extends(extendedType))
                .Where(method => method != null);
        }

        internal static bool IsEnum(string enumName)
        {
            return enumName != null && (UserDefinedTypes.TryGetValue(enumName, out Type type) || AllTypes.TryGetValue(enumName, out type)) &&
                type.IsEnum && Enum.GetValues(type).Length > 0;
        }

        internal static bool TryGetEnumNames(string enumName, out string[] names)
        {
            if (enumName == null || !(UserDefinedTypes.TryGetValue(enumName, out Type type) || AllTypes.TryGetValue(enumName, out type)) || !type.IsEnum)
            {
                names = null;
                return false;
            }

            names = Enum.GetNames(type).Select(name => type.Name + "." + name).ToArray();
            return true;
        }

        internal static void SetReferences(ProjectType projectType)
        {
            List<string> pdnAssemblyNames = new List<string>
            {
                "PaintDotNet.Base",
                "PaintDotNet.ComponentModel",
                "PaintDotNet.Core",
                "PaintDotNet.Data",
                "PaintDotNet.Framework",
                "PaintDotNet.Fundamentals",
                "PaintDotNet.ObjectModel",
                "PaintDotNet.Primitives",
                "PaintDotNet.PropertySystem"
            };

            List<string> intelliIgnoreList = new List<string>
            {
                "PresentationCore",
                "PresentationFramework",
                "WindowsBase"
            };

            switch (projectType)
            {
                case ProjectType.ClassicEffect:
                    pdnAssemblyNames.Add("PaintDotNet.Effects");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Core");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Legacy");
                    break;
                case ProjectType.GpuDrawEffect:
                case ProjectType.GpuEffect:
                case ProjectType.BitmapEffect:
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Core");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Gpu");
                    pdnAssemblyNames.Add("PaintDotNet.Windows");
                    pdnAssemblyNames.Add("PaintDotNet.Windows.Core");
                    pdnAssemblyNames.Add("PaintDotNet.Windows.Framework");

                    intelliIgnoreList.Add("System.Drawing.Common");
                    intelliIgnoreList.Add("System.Drawing.Primitives");
                    break;
                case ProjectType.Reference:
                    pdnAssemblyNames.Add("PaintDotNet.Effects");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Core");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Gpu");
                    pdnAssemblyNames.Add("PaintDotNet.Effects.Legacy");
                    pdnAssemblyNames.Add("PaintDotNet.Windows");
                    pdnAssemblyNames.Add("PaintDotNet.Windows.Core");
                    pdnAssemblyNames.Add("PaintDotNet.Windows.Framework");
                    break;
            }

            PdnAssemblyNames = pdnAssemblyNames
                .Append("PaintDotNet.Windows") // PaintDotNet.Windows needed for the Classic Effect constructor that uses a System.Drawing.Bitmap
                .Distinct()
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToImmutableArray();

            ImmutableArray<Assembly> allAssemblies = sdkAssemblies
                .Concat(allPdnAssemblies.Where(a => pdnAssemblyNames.Contains(a.GetName().Name, StringComparer.Ordinal)))
                .ToImmutableArray();

            ImmutableArray<Assembly> intelliAssemblies = allAssemblies
                .Where(a => !intelliIgnoreList.Contains(a.GetName().Name, StringComparer.Ordinal))
                .ToImmutableArray();

            IEnumerable<string> refFilePaths = allAssemblies
                .Append(allPdnAssemblies.First(a => a.GetName().Name.Equals("PaintDotNet.Windows", StringComparison.Ordinal)))
                .Select(a => a.Location)
                .Distinct();

            ScriptBuilder.SetReferences(refFilePaths);

            AllTypes = new Dictionary<string, Type>(aliasTypes);
            AutoCompleteTypes = new Dictionary<string, Type>(aliasTypes);

            HashSet<string> enums = new HashSet<string>();
            HashSet<string> interfaces = new HashSet<string>();
            HashSet<string> structs = new HashSet<string>()
            {
                "IntSliderControl",
                "CheckboxControl",
                "ColorWheelControl",
                "AngleControl",
                "PanSliderControl",
                "DoubleSliderControl",
                "ListBoxControl",
                "RadioButtonControl",
                "ReseedButtonControl"
            };
            HashSet<string> classes = new HashSet<string>()
            {
                "TextboxControl",
                "FilenameControl",
                "FolderControl",
                "RollControl",
                "MultiLineTextboxControl",
                "LabelComment",
                "FontFamily"
            };

            List<MethodInfo> extMethodsList = new List<MethodInfo>();

            string[] namespaceWhiteList =
            {
                "Microsoft.Win32", "PaintDotNet", "PaintDotNet.AppModel", "PaintDotNet.Effects", "PaintDotNet.Imaging",
                "PaintDotNet.Rendering", "PaintDotNet.Direct2D1", "PaintDotNet.Direct2D1.Effects", "PaintDotNet.Effects.Gpu",
                "System", "System.Collections.Generic", "System.Diagnostics", "System.Drawing", "System.Drawing.Drawing2D",
                "System.Drawing.Text", "System.IO", "System.IO.Compression", "System.Text.RegularExpressions"
            };

            // Add the referenced assembly types
            foreach (Assembly a in intelliAssemblies)
            {
                foreach (Type type in a.GetExportedTypes())
                {
                    string name = (type.IsGenericType) ? Regex.Replace(type.Name, @"`\d", string.Empty) : type.Name;
                    if (!AllTypes.ContainsKey(name))
                    {
                        AllTypes.Add(name, type);
                    }
                    else
                    {
                        Type t = AllTypes[name];
                        if (namespaceWhiteList.Contains(type.Namespace) &&
                            ((type.IsGenericType && !t.IsGenericType) || !namespaceWhiteList.Contains(t.Namespace)))
                        {
                            AllTypes[name] = type;
                        }
                    }

                    if (type.IsNested || type.IsObsolete())
                    {
                        continue;
                    }

                    // Gather extensions methods contained in each type
                    if (type.IsSealed && !type.IsGenericType && type.IsOrHasExtension())
                    {
                        extMethodsList.AddRange(
                            type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                            .Where(method => method.IsOrHasExtension() && !method.IsObsolete()));
                    }
                    else if (namespaceWhiteList.Contains(type.Namespace) &&
                        !type.Name.StartsWith("Property", StringComparison.OrdinalIgnoreCase) &&
                        !AutoCompleteTypes.ContainsKey(type.Name)
                    )
                    {
                        AutoCompleteTypes.Add(type.Name, type);
                    }
                }
            }

            extMethods = extMethodsList;

            if (projectType.Is5Effect())
            {
                // removed to prevent name collision with Environment property
                AllTypes.Remove(nameof(Environment));
                AutoCompleteTypes.Remove(nameof(Environment));
            }

            foreach (KeyValuePair<string, Type> kvp in AllTypes)
            {
                Type type = kvp.Value;
                string name = kvp.Key;

                if (type.IsEnum)
                {
                    enums.Add(name);
                }
                else if (type.IsValueType)
                {
                    structs.Add(name);
                }
                else if (type.IsClass)
                {
                    classes.Add(name);
                }
                else if (type.IsInterface)
                {
                    interfaces.Add(name);
                }
            }

            ClassList = classes.Join(" ");
            EnumList = enums.Join(" ");
            StructList = structs.Join(" ");
            InterfaceList = interfaces.Join(" ");
        }

        static Intelli()
        {
            TypeAliases = new Dictionary<string, string>
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
                { "Void", "void" },
                { "ValueType", "struct" }
            };

            Keywords = new string[]
            {
                "abstract", "as", "base", "break", "case", "catch", "checked", "class", "const", "continue",
                "default", "delegate", "do", "enum", "event", "explicit", "extern", "false", "finally", "fixed",
                "get", "goto", "implicit", "in", "interface", "internal", "is", "lock", "new", "not", "null",
                "operator", "out", "override", "params", "partial", "private", "protected", "public",
                "readonly", "ref", "return", "stackalloc", "static", "sealed", "set", "sizeof", "struct",
                "this", "throw", "true", "try", "typeof", "unchecked", "unsafe", "var", "virtual", "void", "volatile",
                "with", "#endif", "#endregion"
            };

            string[] allPdnAssemblyNames = new string[]
            {
                "PaintDotNet.Base",
                "PaintDotNet.ComponentModel",
                "PaintDotNet.Core",
                "PaintDotNet.Data",
                "PaintDotNet.Effects",
                "PaintDotNet.Effects.Core",
                "PaintDotNet.Effects.Gpu",
                "PaintDotNet.Effects.Legacy",
                "PaintDotNet.Framework",
                "PaintDotNet.Fundamentals",
                "PaintDotNet.ObjectModel",
                "PaintDotNet.Primitives",
                "PaintDotNet.PropertySystem",
                "PaintDotNet.Windows",
                "PaintDotNet.Windows.Core",
                "PaintDotNet.Windows.Framework"
            };

            // exclude assemblies that were loaded into separate contexts; i.e. Plugins
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsCollectible);

            // Cherry pick certain dotPDN assemblies
            allPdnAssemblies = assemblies
                .Where(a => a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company == "dotPDN LLC" &&
                            allPdnAssemblyNames.Contains(a.GetName().Name, StringComparer.OrdinalIgnoreCase))
                .ToImmutableArray();

            // Cherry pick Microsoft assemblies
            sdkAssemblies = assemblies
                .Where(a => a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company == "Microsoft Corporation")
                .Append(typeof(System.Diagnostics.TextWriterTraceListener).Assembly)
                .Distinct()
                .ToImmutableArray();

            Dictionary<string, string> userSnippets = null;
            string userSnippetsJson = Settings.Snippets;

            if (userSnippetsJson.Length > 0)
            {
                try
                {
                    userSnippets = JsonSerializer.Deserialize<Dictionary<string, string>>(userSnippetsJson);
                }
                catch
                {
                }
            }

            Snippets = userSnippets ??
                new Dictionary<string, string>()
                {
                    { "if", "if (true$)\r\n{\r\n    \r\n}" },
                    { "else", "else\r\n{\r\n    $\r\n}" },
                    { "while", "while (true$)\r\n{\r\n    break;\r\n}" },
                    { "for", "for (int i = 0; i < length$; i++)\r\n{\r\n    \r\n}" },
                    { "foreach", "foreach (var item in collection$)\r\n{\r\n    \r\n}" },
                    { "using", "using (resource$)\r\n{\r\n    \r\n}" },
                    { "switch", "switch (variable$)\r\n{\r\n    case 0:\r\n        break;\r\n    default:\r\n        break;\r\n}" },
                    { "#region", "#region MyRegion$\r\n\r\n#endregion" },
                    { "#if", "#if true$\r\n\r\n#endif" },
                    { "try", "try\r\n{\r\n    $\r\n}\r\ncatch (Exception ex)\r\n{\r\n    \r\n}" }
                };

            Variables = new Dictionary<string, Type>();
            Parameters = new Dictionary<string, Type>();
            VarPos = new Dictionary<string, int>();
            UserDefinedTypes = new Dictionary<string, Type>();
            AutoCompleteTypes = new Dictionary<string, Type>();

            aliasTypes = new Dictionary<string, Type>
            {
                // Add the predefined aliases of types in the System namespace
                { "bool", typeof(bool) },
                { "byte", typeof(byte) },
                { "sbyte", typeof(sbyte) },
                { "char", typeof(char) },
                { "decimal", typeof(decimal) },
                { "double", typeof(double) },
                { "float", typeof(float) },
                { "int", typeof(int) },
                { "uint", typeof(uint) },
                { "long", typeof(long) },
                { "ulong", typeof(ulong) },
                { "object", typeof(object) },
                { "short", typeof(short) },
                { "ushort", typeof(ushort) },
                { "string", typeof(string) },
                // Add the aliases for the UI controls
                { "IntSliderControl", typeof(int) },
                { "CheckboxControl", typeof(bool) },
                { "ColorWheelControl", typeof(ColorBgra) },
                { "AngleControl", typeof(double) },
                { "PanSliderControl", typeof(Vector2Double) },
                { "TextboxControl", typeof(string) },
                { "FilenameControl", typeof(string) },
                { "FolderControl", typeof(string) },
                { "DoubleSliderControl", typeof(double) },
                { "RollControl", typeof(Vector3Double) },
                { "ListBoxControl", typeof(byte) },
                { "RadioButtonControl", typeof(byte) },
                { "ReseedButtonControl", typeof(byte) },
                { "MultiLineTextboxControl", typeof(string) }
            }
            .ToImmutableArray();

            SetReferences(ProjectType.Default);

            XamlAutoCompleteTypes = new Dictionary<string, Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(System.Windows.Media.Geometry)).GetExportedTypes())
            {
                if (type.IsNested || type.IsObsolete())
                {
                    continue;
                }

                if (type.IsClass &&
                    type.Namespace.Equals("System.Windows.Media", StringComparison.OrdinalIgnoreCase) &&
                    !XamlAutoCompleteTypes.ContainsKey(type.Name)
                )
                {
                    XamlAutoCompleteTypes.Add(type.Name, type);
                }
            }

            UserScript = typeof(CodeLabRegular); // Placeholder effect Type until it's replaced when the UserScript is actually compiled
        }
    }

    internal enum IntelliType
    {
        Method,
        Property,
        Event,
        Field,
        Keyword,
        Type,
        Variable,
        Class,
        Struct,
        Enum,
        Constant,
        EnumItem,
        Snippet,
        Constructor,
        Parameter,
        Interface,
        Delegate,
        None
    }
}
