using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Business.Mutation.ReplaceFinders
{
    public class IfReplaceFinder : CSharpSyntaxRewriter
    {
        public IfReplaceFinder()
        {
            Replacers = new List<Replacer>();
        }

        public List<Replacer> Replacers { get; }

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
                    if (syntaxToken.Kind() == SyntaxKind.EqualsEqualsToken)
                    {
                        var condition =
                            node.Condition.ReplaceToken(tokens.First(), SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken));
                        var newNode = SyntaxFactory.IfStatement(condition, node.Statement);

                        Replacers.Add(new Replacer { Orginal = node, Replace = newNode, Method = method });
                    }

                    if (syntaxToken.Kind() == SyntaxKind.ExclamationEqualsToken)
                    {
                        var condition =
                            node.Condition.ReplaceToken(tokens.First(), SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken));
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