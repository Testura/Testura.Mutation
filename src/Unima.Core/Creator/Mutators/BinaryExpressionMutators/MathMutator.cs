using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Unima.Core.Creator.Mutators.BinaryExpressionMutators
{
    public class MathMutator : BinaryExpressionMutator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public MathMutator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.MinusToken] = SyntaxKind.PlusToken,
                [SyntaxKind.AsteriskToken] = SyntaxKind.SlashToken,
                [SyntaxKind.SlashToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.PercentToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.AmpersandToken] = SyntaxKind.BarToken,
                [SyntaxKind.BarToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.CaretToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.LessThanLessThanToken] = SyntaxKind.GreaterThanGreaterThanToken,
                [SyntaxKind.GreaterThanGreaterThanToken] = SyntaxKind.LessThanLessThanToken
            };
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var operatorKind = node.OperatorToken.Kind();

            if (operatorKind == SyntaxKind.PlusToken)
            {
                var left = node.Left;
                var right = node.Right;

                if (!left.IsKind(SyntaxKind.StringLiteralExpression) && !right.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    var newNode = node.ReplaceToken(node.OperatorToken, SyntaxFactory.Token(SyntaxKind.MinusToken)).NormalizeWhitespace();
                    Replacers.Add(new MutationDocumentDetails(node, newNode, GetWhere(node)));
                }
            }

            return base.VisitBinaryExpression(node);
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}
