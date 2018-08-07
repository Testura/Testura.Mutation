using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations
{
    public class NegateConditionalMutationOperator : BinaryExpressionMutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public NegateConditionalMutationOperator()
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
