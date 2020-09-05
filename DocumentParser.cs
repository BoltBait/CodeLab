using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PaintDotNet.Effects
{
    internal static class DocumentParser
    {
        private static readonly CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);
        private const string prepend = "class UserScript\r\n{\r\n";

        internal static int PosOffset => prepend.Length;

        internal static IEnumerable<string> MethodNames(this ClassDeclarationSyntax classNode)
        {
            IEnumerable<MethodDeclarationSyntax> methods = classNode
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            IEnumerable<string> definedMethods = methods
                .Select(method => method.Identifier.Text);

            IEnumerable<string> referencedMethods = methods
                .SelectMany(meth => meth.DescendantNodes())
                .OfType<InvocationExpressionSyntax>()
                .Select(invocation => invocation.Expression)
                .OfType<MemberAccessExpressionSyntax>()
                .Select(expression => expression.Name.Identifier.Text);


            return definedMethods.Concat(referencedMethods).Distinct();
        }

        internal static IEnumerable<string> VariableAndParameterNames(this ClassDeclarationSyntax classNode)
        {
            IEnumerable<BaseMethodDeclarationSyntax> methods = classNode
                .DescendantNodes()
                .OfType<BaseMethodDeclarationSyntax>();

            IEnumerable<string> variables = methods
                .SelectMany(method => method.DescendantNodes())
                .OfType<StatementSyntax>()
                .SelectMany(statement => statement.ChildNodes())
                .OfType<VariableDeclarationSyntax>()
                .SelectMany(variable => variable.Variables)
                .Select(variable => variable.Identifier.Text);

            IEnumerable<string> parameters = methods
                .SelectMany(method => method.ParameterList.Parameters)
                .Select(parameter => parameter.Identifier.Text);

            return variables.Concat(parameters).Distinct();
        }

        internal static IEnumerable<ParameterSyntax> MethodParameters(this BaseMethodDeclarationSyntax methodNode)
        {
            return methodNode.ParameterList.Parameters;
        }

        internal static IEnumerable<VariableDeclarationSyntax> VarsForPosition(this BaseMethodDeclarationSyntax methodNode, int position)
        {
            SyntaxNode currentNode = methodNode.GetCurrentNode(position);

            IEnumerable<SyntaxNode> ancestorNodes = currentNode.AncestorsAndSelf().OfType<BlockSyntax>();
            IEnumerable<SyntaxNode> ancestorsNodesInMember = ancestorNodes.Intersect(methodNode.DescendantNodes());

            if (!ancestorsNodesInMember.Any())
            {
                return Array.Empty<VariableDeclarationSyntax>();
            }

            IEnumerable<ForStatementSyntax> forStatementsInCurrentNode = ancestorNodes.First().ChildNodes().OfType<ForStatementSyntax>().Where(statement => !statement.Span.Contains(position));

            return ancestorsNodesInMember
                .SelectMany(node => node.ChildNodes())
                .Except(forStatementsInCurrentNode)
                .OfType<StatementSyntax>()
                .SelectMany(statement => statement.ChildNodes())
                .OfType<VariableDeclarationSyntax>();
        }

        internal static ClassDeclarationSyntax GetRootClassNode(string userCode)
        {
            string classCode = prepend + userCode + "\r\n}";

            return CSharpSyntaxTree.ParseText(classCode, options: parseOptions)
                .GetRoot()
                .ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .First();
        }

        internal static BaseMethodDeclarationSyntax GetCurrentMethod(string userCode, int position)
        {
            return GetRootClassNode(userCode)
                .GetCurrentNode<BaseMethodDeclarationSyntax>(position);
        }

        internal static T GetCurrentNode<T>(this SyntaxNode parentNode, int position)
            where T : CSharpSyntaxNode
        {
            SyntaxNode currentNode = parentNode;
            T methodNode = null;

            while (true)
            {
                SyntaxNodeOrToken child = currentNode.ChildThatContainsPosition(position);

                if (!child.IsNode)
                {
                    break;
                }

                currentNode = child.AsNode();

                if (currentNode is T methodDeclaration)
                {
                    methodNode = methodDeclaration;
                }
            }

            return methodNode;
        }

        internal static SyntaxNode GetCurrentNode(this SyntaxNode parentNode, int position)
        {
            SyntaxNode currentNode = parentNode;

            while (true)
            {
                SyntaxNodeOrToken child = currentNode.ChildThatContainsPosition(position);

                if (!child.IsNode)
                {
                    break;
                }

                currentNode = child.AsNode();
            }

            if ((currentNode.HasLeadingTrivia && currentNode.GetLeadingTrivia().Span.Contains(position)) ||
                (currentNode.HasTrailingTrivia && currentNode.GetTrailingTrivia().Span.Contains(position)))
            {
                currentNode = currentNode.Parent;
            }

            return currentNode;
        }

        internal static Type BuildType(this TypeSyntax typeSyntax)
        {
            string typeString;

            if (typeSyntax is GenericNameSyntax genericType)
            {
                typeString = genericType.Identifier.Text;
            }
            else if (typeSyntax is ArrayTypeSyntax arrayType)
            {
                typeString = arrayType.ElementType.ToString();
            }
            else
            {
                typeString = typeSyntax.ToString();
            }

            Type type;
            if (!Intelli.AllTypes.TryGetValue(typeString, out type) &&
                !Intelli.UserDefinedTypes.TryGetValue(typeString, out type))
            {
                return null;
            }

            if (typeSyntax is GenericNameSyntax genericType2)
            {
                string args = genericType2.TypeArgumentList.Arguments.ToString();
                return type.MakeGenericType(args);
            }

            if (typeSyntax is ArrayTypeSyntax)
            {
                return type.MakeArrayType();
            }

            return type;
        }
    }
}
