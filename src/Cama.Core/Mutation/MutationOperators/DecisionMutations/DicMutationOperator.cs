using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Replacer = Cama.Core.Models.Mutation.Replacer;

namespace Cama.Core.Mutation.MutationOperators.DecisionMutations
{
    public class DicMutationOperator : MutationOperator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _replacementTable;

        public DicMutationOperator()
        {
            _replacementTable = new Dictionary<SyntaxKind, SyntaxKind>
            {
                [SyntaxKind.EqualsEqualsToken] = SyntaxKind.ExclamationEqualsToken,
                [SyntaxKind.ExclamationEqualsToken] = SyntaxKind.EqualsEqualsToken,
                [SyntaxKind.LessThanExpression] = SyntaxKind.GreaterThanExpression,
                [SyntaxKind.LessThanOrEqualExpression] = SyntaxKind.GreaterThanExpression,
                [SyntaxKind.GreaterThanExpression] = SyntaxKind.LessThanExpression,
                [SyntaxKind.GreaterThanOrEqualExpression] = SyntaxKind.LessThanExpression
            };
        }

        public override SyntaxNode VisitIfStatement(IfStatementSyntax node)
        {
            MethodDeclarationSyntax method = null;
            var parent = node.Parent;
            while (parent.GetType() != typeof(MethodDeclarationSyntax) && parent.GetType() != typeof(ConstructorDeclarationSyntax))
            {
                parent = parent.Parent;
            }

            if (parent?.GetType() == typeof(MethodDeclarationSyntax))
            {
                method = parent as MethodDeclarationSyntax;
            }

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

                        Replacers.Add(new Replacer { Orginal = node, Replace = newNode, Method = method });
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed on: " + node);
            }

            return node;
        }
    }
}