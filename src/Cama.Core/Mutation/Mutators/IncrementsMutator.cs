using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators
{
    public class IncrementsMutator : MutationOperator
    {
        public override SyntaxNode VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.PostIncrementExpression) && node.Parent?.Kind() != SyntaxKind.ForStatement)
            {
                var newNode = SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, node.Operand);
                CreateReplacer(node, newNode);
            }

            if (node.IsKind(SyntaxKind.PostDecrementExpression) && node.Parent?.Kind() != SyntaxKind.ForStatement)
            {
                var newNode = SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, node.Operand);
                CreateReplacer(node, newNode);
            }

            return base.VisitPostfixUnaryExpression(node);
        }

        private void CreateReplacer(PostfixUnaryExpressionSyntax node, PostfixUnaryExpressionSyntax newNode)
        {
            var orginalStatement = GetStatement(node);
            var mutatesdStatement = orginalStatement.ReplaceNode(node, newNode);

            Replacers.Add(new Replacer(orginalStatement, mutatesdStatement, GetWhere(node)));
        }
    }
}
