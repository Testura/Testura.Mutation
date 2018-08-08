using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.Mutators.BinaryExpressionMutators
{
    public class MathMutator : BinaryExpressionMutator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public MathMutator()
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
                [SyntaxKind.GreaterThanGreaterThanToken] = SyntaxKind.LessThanLessThanToken
            };
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}
