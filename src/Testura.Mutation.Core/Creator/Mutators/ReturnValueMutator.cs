using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public class ReturnValueMutator : Mutator
    {
        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            if (!node.DescendantNodes().Any())
            {
                return base.VisitReturnStatement(node);
            }

            if (node.DescendantNodes().First() is LiteralExpressionSyntax literlaExpression)
            {
                SyntaxNode newNode = null;

                if (literlaExpression.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression, SyntaxFactory.Token(SyntaxKind.FalseKeyword)));
                }

                if (literlaExpression.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression, SyntaxFactory.Token(SyntaxKind.TrueKeyword)));
                }

                if (literlaExpression.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    newNode = node.Parent.ReplaceNode(node.Parent, SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName($"System.{typeof(Exception).Name}")).WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Mmmmutation"))))))));
                }

                if (literlaExpression.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Mutation")));
                }

                if (literlaExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    if (double.TryParse(literlaExpression.Token.Value.ToString(), out var value))
                    {
                        value = value == 0 ? 1 : 0;
                        newNode = node.ReplaceNode(literlaExpression, SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value)));
                    }
                }

                if (newNode != null)
                {
                    newNode = newNode.NormalizeWhitespace();
                    Replacers.Add(new MutationDocumentDetails(node, newNode, GetWhere(node)));
                }
            }

            var objectCreationExpressions = node.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();
            foreach (var objectCreationExpressionSyntax in objectCreationExpressions)
            {
                var newNode = node.ReplaceNode(objectCreationExpressionSyntax, SyntaxFactory.DefaultExpression(objectCreationExpressionSyntax.Type));
                Replacers.Add(new MutationDocumentDetails(node, newNode, GetWhere(node)));
            }

            return base.VisitReturnStatement(node);
        }
    }
}
