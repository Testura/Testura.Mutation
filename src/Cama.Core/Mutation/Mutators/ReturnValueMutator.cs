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
            if (!node.DescendantNodes().Any())
            {
                return base.VisitReturnStatement(node);
            }

            if (node.DescendantNodes().First() is LiteralExpressionSyntax literlaExpression)
            {
                SyntaxNode newNode = null;

                if (literlaExpression.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword)));
                }

                if (literlaExpression.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, LiteralExpression(SyntaxKind.TrueLiteralExpression, Token(SyntaxKind.TrueKeyword)));
                }

                if (literlaExpression.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    newNode = node.Parent.ReplaceNode(node.Parent, ThrowStatement(ObjectCreationExpression(IdentifierName(typeof(Exception).Name)).WithArgumentList(ArgumentList(SingletonSeparatedList<ArgumentSyntax>(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Mmmmutation"))))))));
                }

                if (literlaExpression.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    newNode = node.ReplaceNode(literlaExpression, LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("Mutation")));
                }

                if (literlaExpression.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    if (double.TryParse(literlaExpression.Token.Value.ToString(), out var value))
                    {
                        value = value == 0 ? 1 : 0;
                        newNode = node.ReplaceNode(literlaExpression, LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value)));
                    }
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
