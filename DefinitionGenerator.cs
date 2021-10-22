/////////////////////////////////////////////////////////////////////////////////
// CodeLab for Paint.NET
// Copyright 2020 Jason Wendt. All Rights Reserved.
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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects
{
    internal static class DefinitionGenerator
    {
        private static readonly StringBuilder defRef = new StringBuilder();
        private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static int indent = 1;
        private static int indentSpaces = 4;

        internal static string Generate(Type type)
        {
            defRef.Clear();

            defRef.AppendLine("namespace " + type.Namespace);
            defRef.AppendLine("{");

            indentSpaces = Settings.IndentSpaces;

            indent = 1;
            string spaces = GetIndent(indent);

            if (type.IsDelegate())
            {
                MethodInfo method = type.GetMethod("Invoke");
                defRef.AppendLine(spaces + "public delegate " + method.ReturnType.GetDisplayName() + " " + type.GetDisplayNameWithExclusion(type) + "(" + method.Params() + ");");
                defRef.AppendLine("}");

                return defRef.ToString();
            }

            if (type.IsEnum && type.IsDefined(typeof(FlagsAttribute)))
            {
                defRef.AppendLine(spaces + "[Flags]");
            }

            defRef.AppendLine(spaces + "public " + type.GetModifiers() + type.GetObjectType() + " " + type.GetDisplayNameWithExclusion(type) + type.GetInheritance());
            if (type.IsGenericType)
            {
                foreach (string constraint in type.GetGenericArguments().GetConstraints())
                {
                    defRef.AppendLine(spaces + "    " + constraint);
                }
            }
            defRef.AppendLine(spaces + "{");
            indent++;

            IterateMembers(type);

            indent--;
            defRef.AppendLine(GetIndent(indent) + "}");
            defRef.AppendLine("}");

            return defRef.ToString();
        }

        private static void IterateMembers(Type type)
        {
            bool isInterface = type.IsInterface;
            string spaces = GetIndent(indent);

            List<FieldInfo> fields = type.GetFields(bindingFlags).ToList();
            fields.Sort(FieldCompare);

            bool areFields = false;
            foreach (FieldInfo field in fields)
            {
                if (field.IsNotVisible() || field.IsSpecialName || field.IsObsolete())
                {
                    continue;
                }

                string access = isInterface ? string.Empty : field.GetAccessModifiers();

                if (field.FieldType.IsEnum)
                {
                    defRef.AppendLine(spaces + field.Name + " = " + field.GetEnumValue() + ",");
                }
                else
                {
                    areFields = true;
                    string value = (field.IsLiteral && !field.IsInitOnly) ? $" = {field.GetConstValue()}" : string.Empty;
                    defRef.AppendLine(spaces + access + field.GetModifiers() + field.FieldType.GetDisplayNameWithExclusion(type) + " " + field.Name + value + ";");
                }
            }

            if (areFields)
            {
                defRef.AppendLine();
            }

            List<ConstructorInfo> ctors = type.GetConstructors(bindingFlags).ToList();
            ctors.Sort(MethodCompare);

            bool areCtors = false;
            foreach (ConstructorInfo ctor in ctors)
            {
                if (ctor.IsNotVisible() || ctor.IsObsolete())
                {
                    continue;
                }

                areCtors = true;

                string access = isInterface ? string.Empty : ctor.GetAccessModifiers();

                defRef.AppendLine(spaces + access + Regex.Replace(type.Name, @"`\d", string.Empty) + "(" + ctor.Params() + ");");
            }

            if (areCtors)
            {
                defRef.AppendLine();
            }

            bool hasDeconstructor = false;
            MethodInfo finalizer = type.GetMethod("Finalize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (finalizer != null)
            {
                hasDeconstructor = true;

                defRef.AppendLine(spaces + "~" + type.Name + "()");
                defRef.AppendLine();
            }

            List<string> indexerProp = new List<string>();
            List<string> regularProp = new List<string>();

            List<PropertyInfo> properties = type.GetProperties(bindingFlags).ToList();
            properties.Sort((x, y) => MethodCompare(x.GetMethod, y.GetMethod));

            foreach (PropertyInfo property in properties)
            {
                MethodInfo propMethod = property.GetMethod;

                if (propMethod.IsNotVisible() || property.IsObsolete())
                {
                    continue;
                }

                string access = isInterface ? string.Empty : propMethod.GetAccessModifiers();
                string modifier = isInterface ? string.Empty : propMethod.GetModifiers();

                ParameterInfo[] indexParams = property.GetIndexParameters();
                if (indexParams.Length > 0)
                {
                    indexerProp.Add(spaces + access + modifier + property.PropertyType.GetDisplayNameWithExclusion(type) + " this[" + indexParams.Select(p => p.BuildParamString()).Join(", ") + "]" + property.GetterSetter());
                }
                else
                {
                    regularProp.Add(spaces + access + modifier + property.PropertyType.GetDisplayNameWithExclusion(type) + " " + property.Name + property.GetterSetter());
                }
            }

            if (indexerProp.Count > 0)
            {
                foreach (string propDef in indexerProp)
                {
                    defRef.AppendLine(propDef);
                }

                defRef.AppendLine();
            }

            if (regularProp.Count > 0)
            {
                foreach (string propDef in regularProp)
                {
                    defRef.AppendLine(propDef);
                }

                defRef.AppendLine();
            }

            bool areEvents = false;
            foreach (EventInfo eventInfo in type.GetEvents(bindingFlags))
            {
                MethodInfo eventMethod = eventInfo.AddMethod;

                if (eventMethod.IsNotVisible() || eventInfo.IsObsolete())
                {
                    continue;
                }

                areEvents = true;

                string access = isInterface ? string.Empty : eventMethod.GetAccessModifiers();
                string modifier = isInterface ? string.Empty : eventMethod.GetModifiers();

                defRef.AppendLine(spaces + access + modifier + "event " + eventInfo.EventHandlerType.GetDisplayName() + " " + eventInfo.Name + ";");
            }

            if (areEvents)
            {
                defRef.AppendLine();
            }

            List<string> opMethods = new List<string>();
            List<string> opImExMethods = new List<string>();
            List<string> otherMethods = new List<string>();

            List<MethodInfo> methods = type.GetMethods(bindingFlags).ToList();
            methods.Sort(MethodCompare);

            foreach (MethodInfo method in methods)
            {
                if (method.IsNotVisible() || method.IsObsolete() ||
                    (method.IsSpecialName && !method.Name.StartsWith("op_", StringComparison.Ordinal)) ||
                    (hasDeconstructor && method.Name.Equals("Finalize", StringComparison.Ordinal)))
                {
                    continue;
                }

                bool isOperator = false;
                bool isImExOperator = false;

                string returnType = method.ReturnType.GetDisplayNameWithExclusion(type);
                string name = method.Name;
                if (method.IsSpecialName && name.StartsWith("op_", StringComparison.Ordinal))
                {
                    isOperator = true;

                    if (name.Equals("op_Equality", StringComparison.Ordinal))
                    {
                        name = returnType + " operator ==";
                    }
                    else if (name.Equals("op_Inequality", StringComparison.Ordinal))
                    {
                        name = returnType + " operator !=";
                    }
                    else if (name.Equals("op_Addition", StringComparison.Ordinal))
                    {
                        name = returnType + " operator +";
                    }
                    else if (name.Equals("op_Subtraction", StringComparison.Ordinal))
                    {
                        name = returnType + " operator -";
                    }
                    else if (name.Equals("op_Implicit", StringComparison.Ordinal))
                    {
                        isImExOperator = true;
                        name = "implicit operator " + returnType;
                    }
                    else if (name.Equals("op_Explicit", StringComparison.Ordinal))
                    {
                        isImExOperator = true;
                        name = "explicit operator " + returnType;
                    }
                    else
                    {
                        name = returnType + " " + name;
                    }
                }
                else
                {
                    string refReturn = method.ReturnType.IsByRef ? "ref " : string.Empty;
                    string nullable = method.IsNullable() ? "?" : string.Empty;
                    name = refReturn + returnType + nullable + " " + name;
                }

                string genericArgs = string.Empty;
                string genericContraints = string.Empty;

                if (method.IsGenericMethod)
                {
                    Type[] args = method.GetGenericArguments();
                    genericArgs = $"<{args.Select(t => t.GetDisplayName()).Join(", ")}>";

                    if (method.IsGenericMethodDefinition)
                    {
                        string constraints = args.GetConstraints().Join("\r\n    " + spaces);
                        if (constraints.Length > 0)
                        {
                            genericContraints = "\r\n    " + spaces + constraints;
                        }
                    }
                }

                string modifier = isInterface ? string.Empty : method.GetModifiers();
                string access = isInterface ? string.Empty : method.GetAccessModifiers();

                string methodDef = spaces + access + modifier + name + genericArgs + "(" + method.Params(false) + ")" + genericContraints + ";";

                if (isImExOperator)
                {
                    opImExMethods.Add(methodDef);
                }
                else if (isOperator)
                {
                    opMethods.Add(methodDef);
                }
                else
                {
                    otherMethods.Add(methodDef);
                }
            }

            if (otherMethods.Count > 0)
            {
                foreach (string methodDef in otherMethods)
                {
                    defRef.AppendLine(methodDef);
                }

                defRef.AppendLine();
            }

            if (opMethods.Count > 0)
            {
                foreach (string methodDef in opMethods)
                {
                    defRef.AppendLine(methodDef);
                }

                defRef.AppendLine();
            }

            if (opImExMethods.Count > 0)
            {
                foreach (string methodDef in opImExMethods)
                {
                    defRef.AppendLine(methodDef);
                }

                defRef.AppendLine();
            }

            foreach (Type nestedType in type.GetNestedTypes(bindingFlags))
            {
                if (!nestedType.IsVisible || nestedType.IsObsolete())
                {
                    continue;
                }

                if (nestedType.IsDelegate())
                {
                    MethodInfo method = nestedType.GetMethod("Invoke");
                    defRef.AppendLine(GetIndent(indent) + "public delegate " + method.ReturnType.GetDisplayName() + " " + nestedType.GetDisplayNameWithExclusion(nestedType) + "(" + method.Params() + ");");

                    continue;
                }

                if (nestedType.IsEnum && nestedType.IsDefined(typeof(FlagsAttribute)))
                {
                    defRef.AppendLine(spaces + "[Flags]");
                }

                defRef.AppendLine(spaces + "public " + nestedType.GetModifiers() + nestedType.GetObjectType() + " " + nestedType.GetDisplayNameWithExclusion(type) + nestedType.GetInheritance());
                if (nestedType.IsGenericType)
                {
                    foreach (string constraint in nestedType.GetGenericArguments().GetConstraints())
                    {
                        defRef.AppendLine(spaces + "    " + constraint);
                    }
                }
                defRef.AppendLine(spaces + "{");
                indent++;

                IterateMembers(nestedType);

                indent--;
                defRef.AppendLine(spaces + "}");
            }
        }

        private static string GetIndent(int indentLevel)
        {
            return new string(' ', indentSpaces * indentLevel);
        }

        private static int MethodCompare(MethodBase x, MethodBase y)
        {
            bool isStaticX = x.IsStatic;
            bool isStaticY = y.IsStatic;

            if (isStaticX != isStaticY)
            {
                return isStaticX ? -1 : 1;
            }

            bool isProtectedInternalX = !x.IsPublic && !x.IsFamily && x.IsFamilyOrAssembly;
            bool isProtectedInternalY = !y.IsPublic && !y.IsFamily && y.IsFamilyOrAssembly;

            if (isProtectedInternalX != isProtectedInternalY)
            {
                return isProtectedInternalX ? 1 : -1;
            }

            bool isProtectedX = !x.IsPublic && x.IsFamily;
            bool isProtectedY = !y.IsPublic && y.IsFamily;

            if (isProtectedX != isProtectedY)
            {
                return isProtectedX ? 1 : -1;
            }

            return x.Name.CompareTo(y.Name);
        }

        private static int FieldCompare(FieldInfo x, FieldInfo y)
        {
            if (x.FieldType.IsEnum)
            {
                return 0;
            }

            bool isConstX = x.IsLiteral && !x.IsInitOnly;
            bool isConstY = y.IsLiteral && !y.IsInitOnly;

            if (isConstX != isConstY)
            {
                return isConstX ? -1 : 1;
            }

            bool isStaticX = x.IsStatic;
            bool isStaticY = y.IsStatic;

            if (isStaticX != isStaticY)
            {
                return isStaticX ? -1 : 1;
            }

            bool isProtectedInternalX = !x.IsPublic && !x.IsFamily && x.IsFamilyOrAssembly;
            bool isProtectedInternalY = !y.IsPublic && !y.IsFamily && y.IsFamilyOrAssembly;

            if (isProtectedInternalX != isProtectedInternalY)
            {
                return isProtectedInternalX ? 1 : -1;
            }

            bool isProtectedX = !x.IsPublic && x.IsFamily;
            bool isProtectedY = !y.IsPublic && y.IsFamily;

            if (isProtectedX != isProtectedY)
            {
                return isProtectedX ? 1 : -1;
            }

            return x.Name.CompareTo(y.Name);
        }

        private static string GetInheritance(this Type type)
        {
            if (type.IsEnum)
            {
                Type baseType = Enum.GetUnderlyingType(type);
                return (baseType != typeof(int)) ? " : " + baseType.GetDisplayName() : string.Empty;
            }

            IEnumerable<Type> directInterfaces = type.GetDirectBaseInterfaces().Where(i => i.IsVisibleInterface());
            IEnumerable<Type> inheritedInterfaces = directInterfaces.SelectMany(i => i.GetInterfaces());

            IEnumerable<string> allInheritance = directInterfaces.Concat(inheritedInterfaces)
                .Distinct()
                .Select(i => i.GetDisplayName())
                .OrderBy(s => s);

            if (type.IsClass && type != typeof(object) && type.BaseType != typeof(object))
            {
                allInheritance = allInheritance.Prepend(type.BaseType.GetDisplayName());
            }

            return allInheritance.Any()
                ? " : " + allInheritance.Join(", ")
                : string.Empty;
        }

        private static bool IsVisibleInterface(this Type type)
        {
            if (type.IsPublic)
            {
                return true;
            }

            if (type.GetCustomAttribute<InterfaceTypeAttribute>()?.Value == ComInterfaceType.InterfaceIsIUnknown)
            {
                return false;
            }

            if (type.IsNotPublic && type.GetInterfaces().Length == 0)
            {
                return false;
            }

            return true;
        }

        private static string GetModifiers(this Type type)
        {
            if (type.IsEnum)
            {
                return string.Empty;
            }

            if (type.IsAbstract && type.IsSealed)
            {
                return "static ";
            }

            if (type.IsAbstract && !type.IsInterface)
            {
                return "abstract ";
            }

            if (type.IsSealed && !type.IsValueType)
            {
                return "sealed ";
            }

            return string.Empty;
        }

        private static string GetModifiers(this FieldInfo field)
        {
            if (field.FieldType.IsEnum)
            {
                return string.Empty;
            }

            if (!field.IsStatic && field.IsInitOnly)
            {
                return "readonly ";
            }

            if (field.IsLiteral && !field.IsInitOnly)
            {
                return "const ";
            }

            if (field.IsStatic && field.IsInitOnly)
            {
                return "static readonly ";
            }

            if (field.IsStatic)
            {
                return "static ";
            }

            return string.Empty;
        }

        private static string GetModifiers(this MethodInfo method)
        {
            if (method.IsStatic)
            {
                return "static ";
            }

            if (method.IsAbstract)
            {
                return "abstract ";
            }

            if (method.IsVirtual && method != method.GetBaseDefinition())
            {
                return method.IsFinal ? "sealed override " : "override ";
            }

            if (method.IsVirtual && !method.IsFinal)
            {
                return "virtual ";
            }

            return string.Empty;
        }

        private static string GetAccessModifiers(this FieldInfo field)
        {
            if (!field.IsPublic && !field.IsFamily && field.IsFamilyOrAssembly)
            {
                return "protected internal ";
            }

            if (!field.IsPublic && field.IsFamily)
            {
                return "protected ";
            }

            return "public ";
        }

        private static string GetAccessModifiers(this MethodBase method)
        {
            if (!method.IsPublic && !method.IsFamily && method.IsFamilyOrAssembly)
            {
                return "protected internal ";
            }

            if (!method.IsPublic && method.IsFamily)
            {
                return "protected ";
            }

            return "public ";
        }

        private static bool IsNotVisible(this FieldInfo field)
        {
            return field.IsPrivate || (!field.IsPublic && !field.IsFamily && !field.IsFamilyOrAssembly);
        }

        private static bool IsNotVisible(this MethodBase method)
        {
            return method.IsPrivate || (!method.IsPublic && !method.IsFamily && !method.IsFamilyOrAssembly);
        }
    }
}
