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
// Latest distribution: http://www.BoltBait.com/pdn/codelab
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class Intelli
    {
        internal static Dictionary<string, Type> Variables { get; }
        internal static Dictionary<string, int> VarPos { get; }
        internal static Dictionary<string, Type> Parameters { get; }
        internal static Dictionary<string, Type> UserDefinedTypes { get; }
        internal static Dictionary<string, Type> AutoCompleteTypes { get; }
        internal static Dictionary<string, Type> AllTypes { get; }
        internal static Dictionary<string, string> Snippets { get; }
        internal static string[] Keywords { get; }
        internal static Type UserScript
        {
            get
            {
                return userScript;
            }
            set
            {
                userScript = value;
                Intelli.Parameters.Clear();
                foreach (MethodInfo method in userScript.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (method.IsVirtual)
                    {
                        continue;
                    }

                    foreach (ParameterInfo parameter in method.GetParameters())
                    {
                        if (!Intelli.Parameters.ContainsKey(parameter.Name))
                        {
                            Intelli.Parameters.Add(parameter.Name, parameter.ParameterType);
                        }
                    }
                }
            }
        }
        private static Type userScript;

        static Intelli()
        {
            Keywords = new string[]
            {
                "abstract", "as", "base", "break", "case", "catch", "checked", "class", "const", "continue",
                "default", "delegate", "do", "enum", "event", "explicit", "extern", "false", "finally", "fixed",
                "get", "goto", "implicit", "in", "interface", "internal", "is", "lock", "new", "null",
                "object", "operator", "out", "override", "params", "partial", "private", "protected", "public",
                "readonly", "ref", "return", "stackalloc", "static", "sealed", "set", "sizeof", "struct",
                "this", "throw", "true", "try", "typeof", "unchecked", "unsafe", "var", "virtual", "void", "volatile",
                "#endif", "#endregion"
            };

            Snippets = new Dictionary<string, string>
            {
                { "if", " (true&)\n{\n    \n}" },
                { "else", "\n{\n    &\n}" },
                { "while", " (false&)\n{\n    \n}" },
                { "for", " (int i = 0; i < length&; i++)\n{\n    \n}" },
                { "foreach", " (var item in collection&)\n{\n    \n}" },
                { "using", " (resource&)\n{\n    \n}" },
                { "switch", " (variable&)\n{\n    case 0:\n        break;\n    default:\n        break;\n}" },
                { "#region", " MyRegion&\n\n#endregion" },
                { "#if", " true&\n\n#endif" }
            };

            Variables = new Dictionary<string, Type>();

            Parameters = new Dictionary<string, Type>();

            VarPos = new Dictionary<string, int>();

            UserDefinedTypes = new Dictionary<string, Type>();

            AllTypes = new Dictionary<string, Type>();

            // Add the predefined aliases of types in the System namespace
            AllTypes.Add("bool", typeof(bool));
            AllTypes.Add("byte", typeof(byte));
            AllTypes.Add("sbyte", typeof(sbyte));
            AllTypes.Add("char", typeof(char));
            AllTypes.Add("decimal", typeof(decimal));
            AllTypes.Add("double", typeof(double));
            AllTypes.Add("float", typeof(float));
            AllTypes.Add("int", typeof(int));
            AllTypes.Add("uint", typeof(uint));
            AllTypes.Add("long", typeof(long));
            AllTypes.Add("ulong", typeof(ulong));
            AllTypes.Add("object", typeof(object));
            AllTypes.Add("short", typeof(short));
            AllTypes.Add("ushort", typeof(ushort));
            AllTypes.Add("string", typeof(string));

            AllTypes.Add("IntSliderControl", typeof(int));
            AllTypes.Add("CheckboxControl", typeof(bool));
            AllTypes.Add("ColorWheelControl", typeof(ColorBgra));
            AllTypes.Add("AngleControl", typeof(double));
            AllTypes.Add("PanSliderControl", typeof(Pair<double, double>));
            AllTypes.Add("TextboxControl", typeof(string));
            AllTypes.Add("DoubleSliderControl", typeof(double));
            AllTypes.Add("RollControl", typeof(Tuple<double, double, double>));
            AllTypes.Add("ListBoxControl", typeof(byte));
            AllTypes.Add("RadioButtonControl", typeof(byte));
            AllTypes.Add("ReseedButtonControl", typeof(byte));
            AllTypes.Add("MultiLineTextboxControl", typeof(string));

            AutoCompleteTypes = new Dictionary<string, Type>(AllTypes);

            AllTypes.Add("bool[]", typeof(bool).MakeArrayType());
            AllTypes.Add("byte[]", typeof(byte).MakeArrayType());
            AllTypes.Add("sbyte[]", typeof(sbyte).MakeArrayType());
            AllTypes.Add("char[]", typeof(char).MakeArrayType());
            AllTypes.Add("decimal[]", typeof(decimal).MakeArrayType());
            AllTypes.Add("double[]", typeof(double).MakeArrayType());
            AllTypes.Add("float[]", typeof(float).MakeArrayType());
            AllTypes.Add("int[]", typeof(int).MakeArrayType());
            AllTypes.Add("uint[]", typeof(uint).MakeArrayType());
            AllTypes.Add("long[]", typeof(long).MakeArrayType());
            AllTypes.Add("ulong[]", typeof(ulong).MakeArrayType());
            AllTypes.Add("object[]", typeof(object).MakeArrayType());
            AllTypes.Add("short[]", typeof(short).MakeArrayType());
            AllTypes.Add("ushort[]", typeof(ushort).MakeArrayType());
            AllTypes.Add("string[]", typeof(string).MakeArrayType());

            // Add the referenced assembly types
            List<AssemblyName> assemblies = new List<AssemblyName>();
            assemblies.Add(typeof(Effect).Assembly.GetName());
            assemblies.AddRange(typeof(Effect).Assembly.GetReferencedAssemblies());

            foreach (AssemblyName a in assemblies)
            {
                if (a.Name.Equals("PaintDotNet.Framework") || a.Name.Equals("PaintDotNet.Resources") || a.Name.Equals("PaintDotNet.SystemLayer"))
                {
                    continue;
                }

                Assembly subjectAssembly = Assembly.Load(a);
                Type[] assemblyTypes = subjectAssembly.GetExportedTypes();

                foreach (Type type in assemblyTypes)
                {
                    string name = (type.IsGenericType) ? Regex.Replace(type.Name, @"`\d", string.Empty) : type.Name;
                    if (!AllTypes.ContainsKey(name))
                    {
                        AllTypes.Add(name, type);
                    }

                    try
                    {
                        Type arrayType = type.MakeArrayType();
                        if (!AllTypes.ContainsKey(arrayType.Name))
                        {
                            AllTypes.Add(arrayType.Name, arrayType);
                        }
                    }
                    catch
                    {
                    }

                    if ((type.Namespace == "Microsoft.Win32" || type.Namespace == "PaintDotNet" || type.Namespace == "PaintDotNet.Effects" || type.Namespace == "System" ||
                        type.Namespace == "System.Collections.Generic" || type.Namespace == "System.Diagnostics" || type.Namespace == "System.IO.Compression" ||
                        type.Namespace == "System.Drawing" || type.Namespace == "System.Drawing.Drawing2D" || type.Namespace == "System.Drawing.Text" ||
                        type.Namespace == "System.IO" || type.Namespace == "System.Linq" || type.Namespace == "System.Text.RegularExpressions") &&
                        !type.IsInterface && !type.Name.StartsWith("Property", StringComparison.OrdinalIgnoreCase) && !type.IsObsolete(false) &&
                        !AutoCompleteTypes.ContainsKey(type.Name)
                    )
                    {
                        AutoCompleteTypes.Add(type.Name, type);
                    }
                }
            }
        }
    }

    internal enum IntelliTypes
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
        None
    }
}
