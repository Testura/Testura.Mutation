using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators
{
    public class NegateTypeCompabilityMutator : Mutator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.IsExpression))
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node));
                Replacers.Add(new Replacer(node, newNode, GetWhere(node)));
            }

            return base.VisitBinaryExpression(node);
        }
    }
}
