using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public class ReturnValueMutator : Mutator
    {
        protected override MutationOperators Category => MutationOperators.ReturnValue;

        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (!node.DescendantNodes().Any())
            {
                return base.VisitReturnStatement(node);
            }

            if (node.DescendantNodes().First() is LiteralExpressionSyntax literalExpressionSyntax)
            {
                SyntaxNode newNode = null;

                if (literalExpressionSyntax.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression, SyntaxFactory.Token(SyntaxKind.FalseKeyword)));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression, SyntaxFactory.Token(SyntaxKind.TrueKeyword)));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    newNode = node.Parent.ReplaceNode(node.Parent, SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName($"System.{typeof(Exception).Name}")).WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Mmmmutation"))))))));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Mutation")));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    if (double.TryParse(literalExpressionSyntax.Token.Value.ToString(), out var value))
                    {
                        value = value == 0 ? 1 : 0;
                        newNode = node.ReplaceNode(literalExpressionSyntax, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value)));
                    }
                }

                if (newNode != null)
                {
                    newNode = newNode.NormalizeWhitespace();
                    Replacers.Add(new MutationDocumentDetails(
                        node,
                        newNode,
                        GetWhere(node),
                        CreateCategory(literalExpressionSyntax.Kind().ToString())));
                }
            }

            var objectCreationExpressions = node.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();
            foreach (var objectCreationExpressionSyntax in objectCreationExpressions)
            {
                var newNode = node.ReplaceNode(objectCreationExpressionSyntax, SyntaxFactory.DefaultExpression(objectCreationExpressionSyntax.Type));
                Replacers.Add(
                    new MutationDocumentDetails(
                        node,
                        newNode,
                        GetWhere(node),
                        CreateCategory("ObjectCreation")));
            }

            return base.VisitReturnStatement(node);
        }
    }
}
