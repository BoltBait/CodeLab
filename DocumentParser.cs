using System;
using System.Collections.Generic;
using System.Linq;
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
                .OfType<VariableDeclaratorSyntax>()
                .Select(variable => variable.Identifier.Text);

            IEnumerable<string> variables2 = methods
                .SelectMany(method => method.DescendantNodes())
                .OfType<SingleVariableDesignationSyntax>()
                .Select(variable => variable.Identifier.Text);

            IEnumerable<string> parameters = methods
                .SelectMany(method => method.ParameterList.Parameters)
                .Select(parameter => parameter.Identifier.Text);

            return variables.Concat(variables2).Concat(parameters).Distinct();
        }

        internal static IEnumerable<ParameterSyntax> MethodParameters(this BaseMethodDeclarationSyntax methodNode)
        {
            return methodNode.ParameterList.Parameters;
        }

        internal static IEnumerable<VariableDeclarationSyntax> VarsForPosition(this BaseMethodDeclarationSyntax methodNode, int position)
        {
            SyntaxNode currentNode = methodNode.GetCurrentNode(position);

            IEnumerable<BlockSyntax> ancestorNodes = currentNode.AncestorsAndSelf().OfType<BlockSyntax>();
            IEnumerable<BlockSyntax> ancestorsNodesInMember = ancestorNodes.Intersect(methodNode.DescendantNodes().OfType<BlockSyntax>());

            if (!ancestorsNodesInMember.Any())
            {
                return Array.Empty<VariableDeclarationSyntax>();
            }

            BlockSyntax firstBlockSyntax = ancestorsNodesInMember.First();

            IEnumerable<VariableDeclarationSyntax> othersInCurrentNode = firstBlockSyntax
                .ChildNodes()
                .OfType<StatementSyntax>()
                .Where(statement => statement.Span.Contains(position))
                .SelectMany(statement => statement.DescendantNodes())
                .OfType<VariableDeclarationSyntax>();

            IEnumerable<VariableDeclarationSyntax> locals = ancestorsNodesInMember
                .SelectMany(blockNode => blockNode.ChildNodes())
                .OfType<LocalDeclarationStatementSyntax>()
                .SelectMany(localStatement => localStatement.DescendantNodes())
                .OfType<VariableDeclarationSyntax>();

            //IEnumerable<SyntaxNode> firstBlockDescendants = firstBlockSyntax.DescendantNodes();
            //ancestorsNodesInMember = ancestorsNodesInMember.Skip(1);

            //return ancestorsNodesInMember
            //    .SelectMany(node => node.ChildNodes())
            //    .OfType<StatementSyntax>()
            //    .Except(forStatementsInCurrentNode)
            //    .SelectMany(statement => statement.DescendantNodes())
            //    .OfType<VariableDeclarationSyntax>()
            //    .Distinct();


            var goo = currentNode
                .AncestorsAndSelf()
                .Intersect(methodNode.DescendantNodes())
                .SelectMany(node => node.ChildNodes())
                .OfType<StatementSyntax>()
                .SelectMany(statement => statement.ChildNodes())
                .OfType<VariableDeclarationSyntax>();

            //var foo = ancestorsNodesInMember
            //    .SelectMany(blockNode => blockNode.ChildNodes())
            //    .Where(statement => statement.Span.Contains(position));

            //var bar = foo.OfType<StatementSyntax>();
            //var buzz = bar.SelectMany(statement => statement.DescendantNodes());
            //var fizz = buzz.Intersect(currentNode.AncestorsAndSelf());
            //var fizz  =   buzz.Where(statement => statement.Span.Contains(position)).Except(ancestorsNodesInMember);
            //.Except(firstBlockDescendants);
            //var goo = fizz.OfType<VariableDeclarationSyntax>()
            //          .Except(locals);
            //.Distinct();


            return goo.Concat(locals);
            //return goo.Concat(localsInCurrentNode).Concat(othersInCurrentNode);
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
            T returnedNode = null;

            while (true)
            {
                SyntaxNodeOrToken child = currentNode.ChildThatContainsPosition(position);

                if (!child.Span.Contains(position) || !child.IsNode)
                {
                    break;
                }

                currentNode = child.AsNode();

                if (currentNode is T tSyntaxNode)
                {
                    returnedNode = tSyntaxNode;
                }
            }

            return returnedNode;
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
