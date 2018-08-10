using System;
using System.Linq;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Cama.Core.Mutation.Mutators
{
    public class ReturnValueMutator : Mutator
    {
        public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
        {
            var literlaExpressions = node.DescendantNodes().OfType<LiteralExpressionSyntax>().ToList();

            foreach (var literalExpressionSyntax in literlaExpressions)
            {
                SyntaxNode newNode = null;

                if (literalExpressionSyntax.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword)));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, LiteralExpression(SyntaxKind.TrueLiteralExpression, Token(SyntaxKind.TrueKeyword)));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    newNode = node.Parent.ReplaceNode(node.Parent, ThrowStatement(ObjectCreationExpression(IdentifierName(typeof(Exception).Name)).WithArgumentList(ArgumentList(SingletonSeparatedList<ArgumentSyntax>(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Mmmmutation"))))))));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    newNode = node.ReplaceNode(literalExpressionSyntax, LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Mutation")));
                }

                if (literalExpressionSyntax.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    if (!double.TryParse(literalExpressionSyntax.Token.Value.ToString(), out var value))
                    {
                        continue;
                    }

                    value = value == 0 ? 1 : 0;
                    newNode = node.ReplaceNode(literalExpressionSyntax, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
                }

                if (newNode != null)
                {
                    Replacers.Add(new Replacer(node, newNode, GetWhere(node)));
                }
            }

            var objectCreationExpressions = node.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().ToList();
            foreach (var objectCreationExpressionSyntax in objectCreationExpressions)
            {
                var newNode = node.ReplaceNode(objectCreationExpressionSyntax, LiteralExpression(SyntaxKind.NullLiteralExpression));
                Replacers.Add(new Replacer(node, newNode, GetWhere(node)));
            }

            return base.VisitReturnStatement(node);
        }
    }
}
