using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations
{
    public class MathMutationOperator : BinaryExpressionMutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public MathMutationOperator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.PlusToken] = SyntaxKind.MinusToken,
                [SyntaxKind.MinusToken] = SyntaxKind.PlusToken,
                [SyntaxKind.AsteriskToken] = SyntaxKind.SlashToken,
                [SyntaxKind.SlashToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.PercentToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.AmpersandToken] = SyntaxKind.BarToken,
                [SyntaxKind.BarToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.CaretToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.LessThanLessThanToken] = SyntaxKind.GreaterThanGreaterThanToken,
                [SyntaxKind.GreaterThanGreaterThanToken] = SyntaxKind.LessThanLessThanToken,
                [SyntaxKind.EqualsEqualsToken] = SyntaxKind.ExclamationEqualsToken,
                [SyntaxKind.ExclamationEqualsToken] = SyntaxKind.EqualsEqualsToken,
                [SyntaxKind.LessThanToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.LessThanEqualsToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.GreaterThanToken] = SyntaxKind.LessThanToken,
                [SyntaxKind.GreaterThanEqualsToken] = SyntaxKind.LessThanToken
            };
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}
