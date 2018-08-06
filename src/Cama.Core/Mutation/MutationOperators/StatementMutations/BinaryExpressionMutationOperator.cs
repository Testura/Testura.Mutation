using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using BinaryExpressionSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax;
using StatementSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax;

namespace Cama.Core.Mutation.MutationOperators.StatementMutations
{
    public class BinaryExpressionMutationOperator : MutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public BinaryExpressionMutationOperator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.PlusToken] = SyntaxKind.MinusToken,
                [SyntaxKind.MinusToken] = SyntaxKind.PlusToken,
                [SyntaxKind.AsteriskToken] = SyntaxKind.SlashToken,
                [SyntaxKind.SlashToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.PercentToken] = SyntaxKind.AsteriskToken,
                [SyntaxKind.AmpersandToken] = SyntaxKind.BarToken,
                [SyntaxKind.BarToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.CaretToken] = SyntaxKind.AmpersandToken,
                [SyntaxKind.LessThanLessThanToken] = SyntaxKind.GreaterThanGreaterThanToken,
                [SyntaxKind.GreaterThanGreaterThanToken] = SyntaxKind.LessThanLessThanToken,
            };
        }

        public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            var operatorKind = node.OperatorToken.Kind();
            if (_replacementTable.ContainsKey(operatorKind))
            {
                var newNode = node.ReplaceToken(node.OperatorToken, SyntaxFactory.Token(_replacementTable[operatorKind]));

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

        private StatementSyntax GetStatement(BinaryExpressionSyntax binaryExpressionSyntax)
        {
            SyntaxNode statementPart = binaryExpressionSyntax.Parent;
            while (!(statementPart is StatementSyntax))
            {
                statementPart = statementPart.Parent;
            }

            return statementPart as StatementSyntax;
        }
    }
}
