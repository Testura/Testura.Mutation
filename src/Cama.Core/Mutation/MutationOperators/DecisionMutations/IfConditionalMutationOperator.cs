using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Replacer = Cama.Core.Models.Mutation.Replacer;

namespace Cama.Core.Mutation.MutationOperators.DecisionMutations
{
    public class IfConditionalMutationOperator : MutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public IfConditionalMutationOperator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.EqualsEqualsToken] = SyntaxKind.ExclamationEqualsToken,
                [SyntaxKind.ExclamationEqualsToken] = SyntaxKind.EqualsEqualsToken,
                [SyntaxKind.LessThanToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.LessThanEqualsToken] = SyntaxKind.GreaterThanToken,
                [SyntaxKind.GreaterThanToken] = SyntaxKind.LessThanToken,
                [SyntaxKind.GreaterThanEqualsToken] = SyntaxKind.LessThanToken
            };
        }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            try
            {
                var tokens = node.Condition.ChildTokens();
                foreach (var syntaxToken in tokens)
                {
                    var kind = syntaxToken.Kind();
                    if (_replacementTable.ContainsKey(kind))
                    {
                        var condition = node.Condition.ReplaceToken(tokens.First(), SyntaxFactory.Token(_replacementTable[kind]));
                        var newNode = SyntaxFactory.IfStatement(condition, node.Statement);

                        Replacers.Add(new Replacer { Orginal = node, Replace = newNode, Where = GetWhere(node) });
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed on: " + node);
            }

            return base.VisitIfStatement(node);
        }
    }
}