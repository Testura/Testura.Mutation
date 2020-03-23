using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators
{
    public abstract class BinaryExpressionMutator : Mutator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var operatorKind = node.OperatorToken.Kind();
            var replacementTable = GetReplacementTable();

            if (replacementTable.ContainsKey(operatorKind))
            {
                var newNode = node.ReplaceToken(node.OperatorToken, SyntaxFactory.Token(replacementTable[operatorKind])).NormalizeWhitespace();
                Replacers.Add(new MutationDocumentDetails(
                    node,
                    newNode,
                    GetWhere(node),
                    CreateCategory(node.OperatorToken.Text)));
            }

            return base.VisitBinaryExpression(node);
        }

        protected abstract Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable();
    }
}
