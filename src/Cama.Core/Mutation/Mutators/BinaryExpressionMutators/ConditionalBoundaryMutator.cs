using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.Mutators.BinaryExpressionMutators
{
    public class ConditionalBoundaryMutator : BinaryExpressionMutator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public ConditionalBoundaryMutator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.LessThanToken] = SyntaxKind.LessThanEqualsToken,
                [SyntaxKind.LessThanEqualsToken] = SyntaxKind.LessThanToken,
                [SyntaxKind.GreaterThanToken] = SyntaxKind.GreaterThanEqualsToken,
                [SyntaxKind.GreaterThanEqualsToken] = SyntaxKind.GreaterThanToken,
            };
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}