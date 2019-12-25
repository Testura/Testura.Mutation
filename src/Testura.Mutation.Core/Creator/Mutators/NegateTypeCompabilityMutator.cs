using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public class NegateTypeCompabilityMutator : Mutator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.IsExpression))
            {
                var newNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(node)).NormalizeWhitespace();
                Replacers.Add(new MutationDocumentDetails(node, newNode, GetWhere(node)));
            }

            return base.VisitBinaryExpression(node);
        }
    }
}
