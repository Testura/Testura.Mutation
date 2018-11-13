using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Creator.Mutators.BinaryExpressionMutators
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

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            if (node.Condition is InvocationExpressionSyntax)
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node.Condition)).NormalizeWhitespace();
                Replacers.Add(new MutationDocumentDetails(node.Condition, newNode, GetWhere(node.Condition)));
            }

            if (node.Condition is PrefixUnaryExpressionSyntax)
            {
                var condition = node.Condition as PrefixUnaryExpressionSyntax;
                Replacers.Add(new MutationDocumentDetails(node.Condition, condition.Operand, GetWhere(condition.Operand)));
            }

            return base.VisitIfStatement(node);
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}