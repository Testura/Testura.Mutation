using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators.BinaryExpressionMutators
{
    public abstract class BinaryExpressionMutator : MutationOperator
    {
        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var operatorKind = node.OperatorToken.Kind();
            var replacementTable = GetReplacementTable();

            if (replacementTable.ContainsKey(operatorKind))
            {
                var newNode = node.ReplaceToken(node.OperatorToken, SyntaxFactory.Token(replacementTable[operatorKind]));

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

        protected abstract Dictionary<SyntaxKind, SyntaxKind> GetReplacementTable();
    }
}
