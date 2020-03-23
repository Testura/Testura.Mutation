using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators
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

        protected override MutationOperators Category => MutationOperators.ConditionalBoundary;

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            if (node.Condition is InvocationExpressionSyntax)
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node.Condition)).NormalizeWhitespace();
                Replacers.Add(new MutationDocumentDetails(
                    node.Condition,
                    newNode,
                    GetWhere(node.Condition),
                    CreateCategory(node.Condition.Kind().ToString())));
            }

            if (node.Condition is PrefixUnaryExpressionSyntax condition)
            {
                Replacers.Add(new MutationDocumentDetails(
                    node.Condition,
                    condition.Operand,
                    GetWhere(condition.Operand),
                    CreateCategory(condition.Kind().ToString())));
            }

            return base.VisitIfStatement(node);
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return _replacementTable;
        }
    }
}