using System.Collections.Generic;
using Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.MutationOperators
{
    public class ConditionalBoundaryMutationOperator : BinaryExpressionMutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public ConditionalBoundaryMutationOperator()
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