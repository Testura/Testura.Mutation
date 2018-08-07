using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations
{
    public class NegateTypeCompabilityMutationOperator : BinaryExpressionMutationOperator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.IsExpression))
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node));

                var orginalStatement = GetStatement(node);
                var mutatesdStatement = orginalStatement.ReplaceNode(node, newNode);

                Replacers.Add(new Replacer
                {
                    Orginal = orginalStatement,
                    Replace = mutatesdStatement,
                    Where = GetWhere(node)
                });
            }

            return base.VisitBinaryExpression(node);
        }

        protected override Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable()
        {
            return new Dictionary<SyntaxKind, SyntaxKind>();
        }
    }
}
