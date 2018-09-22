using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Creator.Mutators.BinaryExpressionMutators
{
    public class NegateConditionalMutator : BinaryExpressionMutator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public NegateConditionalMutator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.EqualsEqualsToken] = SyntaxKind.ExclamationEqualsToken,
                [SyntaxKind.ExclamationEqualsToken] = SyntaxKind.EqualsEqualsToken,
                [SyntaxKind.LessThanToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.LessThanEqualsToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.GreaterThanToken] = SyntaxKind.LessThanToken,
                [SyntaxKind.GreaterThanEqualsToken] = SyntaxKind.LessThanToken,
            };
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}
