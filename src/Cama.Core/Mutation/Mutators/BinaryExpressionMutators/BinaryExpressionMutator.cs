using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators.BinaryExpressionMutators
{
    public abstract class BinaryExpressionMutator : Mutator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var operatorKind = node.OperatorToken.Kind();
            var replacementTable = GetReplacementTable();

            if (replacementTable.ContainsKey(operatorKind))
            {
                var newNode = node.ReplaceToken(node.OperatorToken, SyntaxFactory.Token(replacementTable[operatorKind]).WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
                Replacers.Add(new Replacer(node, newNode, GetWhere(node)));
            }

            return base.VisitBinaryExpression(node);
        }

        protected abstract Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable();
    }
}
