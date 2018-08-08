using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators
{
    public class NegateTypeCompabilityMutator : MutationOperator
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
    }
}
