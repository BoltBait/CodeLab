﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PaintDotNet.Effects
{
    internal static class DefinitionGenerator
    {
        private static readonly StringBuilder defRef = new StringBuilder();
        private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        private static int indent = 1;

        internal static string Generate(Type type)
        {
            defRef.Clear();

            defRef.AppendLine("namespace " + type.Namespace);
            defRef.AppendLine("{");

            indent = 1;

            if (typeof(Delegate).IsAssignableFrom(type))
            {
                MethodInfo method = type.GetMethod("Invoke");
                defRef.AppendLine(GetIndent(indent) + "public delegate " + method.ReturnType.GetDisplayName() + " " + type.GetDisplayNameWithExclusion(type) + "(" + method.Params() + ");");
                defRef.AppendLine("}");

                return defRef.ToString();
            }

            if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() != null)
            {
                defRef.AppendLine(GetIndent(indent) + "[Flags]");
            }

            defRef.AppendLine(GetIndent(indent) + "public " + type.GetModifiers() + type.GetObjectType() + " " + type.GetDisplayNameWithExclusion(type) + type.GetInheritance());
            defRef.AppendLine(GetIndent(indent) + "{");
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

            bool areFields = false;
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                if (field.IsPrivate || (!field.IsPublic && !field.IsFamily) || field.IsSpecialName || field.IsObsolete())
                {
                    continue;
                }

                string access = isInterface ? string.Empty : (!field.IsPublic && field.IsFamily) ? "protected " : "public ";

                if (field.FieldType.IsEnum)
                {
                    defRef.AppendLine(spaces + field.Name + " = " + field.GetEnumValue() + ",");
                }
                else
                {
                    areFields = true;
                    string value = (field.IsLiteral && !field.IsInitOnly) ? $" = {field.GetValue(null)}" : string.Empty;
                    defRef.AppendLine(spaces + access + field.GetModifiers() + field.FieldType.GetDisplayNameWithExclusion(type) + " " + field.Name + value + ";");
                }
            }

            if (areFields)
            {
                defRef.AppendLine();
            }

            bool areCtors = false;
            foreach (ConstructorInfo ctor in type.GetConstructors(bindingFlags))
            {
                if (ctor.IsPrivate || (!ctor.IsPublic && !ctor.IsFamily) || ctor.IsObsolete())
                {
                    continue;
                }

                areCtors = true;

                string access = isInterface ? string.Empty : (!ctor.IsPublic && ctor.IsFamily) ? "protected " : "public ";

                defRef.AppendLine(spaces + access + Regex.Replace(type.Name, @"`\d", string.Empty) + "(" + ctor.Params() + ");");
            }

            if (areCtors)
            {
                defRef.AppendLine();
            }

            bool areProps = false;
            foreach (PropertyInfo property in type.GetProperties(bindingFlags))
            {
                MethodInfo propMethod = property.GetMethod;

                if (propMethod.IsPrivate || (!propMethod.IsPublic && !propMethod.IsFamily) || property.IsObsolete())
                {
                    continue;
                }

                areProps = true;

                string access = isInterface ? string.Empty : (!propMethod.IsPublic && propMethod.IsFamily) ? "protected " : "public ";
                string modifier = isInterface ? string.Empty : propMethod.GetModifiers();

                ParameterInfo[] indexParams = property.GetIndexParameters();
                if (indexParams.Length > 0)
                {
                    defRef.AppendLine(spaces + access + modifier + property.PropertyType.GetDisplayNameWithExclusion(type) + " this[" + indexParams.Select(p => p.BuildParamString()).Join(", ") + "]" + property.GetterSetter());
                }
                else
                {
                    defRef.AppendLine(spaces + access + modifier + property.PropertyType.GetDisplayNameWithExclusion(type) + " " + property.Name + property.GetterSetter());
                }
            }

            if (areProps)
            {
                defRef.AppendLine();
            }

            bool areEvents = false;
            foreach (EventInfo eventInfo in type.GetEvents(bindingFlags))
            {
                MethodInfo eventMethod = eventInfo.AddMethod;

                if (eventMethod.IsPrivate || (!eventMethod.IsPublic && !eventMethod.IsFamily) || eventInfo.IsObsolete())
                {
                    continue;
                }

                areEvents = true;

                string access = isInterface ? string.Empty : (!eventMethod.IsPublic && eventMethod.IsFamily) ? "protected " : "public ";

                defRef.AppendLine(spaces + access + eventMethod.GetModifiers() + "event " + eventInfo.EventHandlerType.GetDisplayName() + " " + eventInfo.Name + ";");
            }

            if (areEvents)
            {
                defRef.AppendLine();
            }

            List<string> staticMethods = new List<string>();
            List<string> opMethods = new List<string>();
            List<string> opImExMethods = new List<string>();
            List<string> otherMethods = new List<string>();

            List<MethodInfo> methods = type.GetMethods(bindingFlags).ToList();
            methods.Sort((a, b) => a.Name.CompareTo(b.Name));

            foreach (MethodInfo method in methods)
            {
                if (method.IsPrivate || (!method.IsPublic && !method.IsFamily) || method.IsObsolete() ||
                    (method.IsSpecialName && !method.Name.StartsWith("op_", StringComparison.Ordinal)))
                {
                    continue;
                }

                string access = isInterface ? string.Empty : (!method.IsPublic && method.IsFamily) ? "protected " : "public ";

                bool isStatic = method.IsStatic;
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
                    name = returnType + " " + name;
                }

                string modifier = isInterface ? string.Empty : method.GetModifiers();
                string methodDef = spaces + access + modifier + name + "(" + method.Params() + ");";

                if (isImExOperator)
                {
                    opImExMethods.Add(methodDef);
                }
                else if (isOperator)
                {
                    opMethods.Add(methodDef);
                }
                else if (isStatic)
                {
                    staticMethods.Add(methodDef);
                }
                else
                {
                    otherMethods.Add(methodDef);
                }
            }

            if (staticMethods.Count > 0 || otherMethods.Count > 0)
            {
                foreach (string methodDef in staticMethods)
                {
                    defRef.AppendLine(methodDef);
                }

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

            int nestedCount = 0;
            foreach (Type nestedType in type.GetNestedTypes(bindingFlags))
            {
                if (!nestedType.IsVisible || nestedType.IsObsolete())
                {
                    continue;
                }

                nestedCount++;

                if (nestedType.IsEnum && nestedType.GetCustomAttribute<FlagsAttribute>() != null)
                {
                    defRef.AppendLine(spaces + "[Flags]");
                }

                defRef.AppendLine(spaces + "public " + nestedType.GetModifiers() + nestedType.GetObjectType() + " " + nestedType.GetDisplayNameWithExclusion(type) + nestedType.GetInheritance());
                defRef.AppendLine(spaces + "{");
                indent++;

                IterateMembers(nestedType);

                indent--;
                defRef.AppendLine(spaces + "}");
            }

            if (nestedCount > 1)
            {
                defRef.AppendLine();
            }
        }

        private static string GetIndent(int indentLevel)
        {
            return new string(' ', 4 * indentLevel);
        }
    }
}