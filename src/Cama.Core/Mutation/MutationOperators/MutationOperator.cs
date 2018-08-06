using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.MutationOperators
{
    public class MutationOperator : CSharpSyntaxRewriter, IMutationOperator
    {
        public MutationOperator()
        {
            Replacers = new List<Replacer>();
        }

        protected IList<Replacer> Replacers { get; }

        public IList<MutatedDocument> GetMutatedDocument(SyntaxNode root, Document document, List<UnitTestInformation> connectedTests)
        {
            Replacers.Clear();
            Visit(root);
            return Replacers.Select(r => new MutatedDocument(document, r, connectedTests)).ToList();
        }

        protected string GetWhere(CSharpSyntaxNode statementSyntax)
        {
            int count = 0;

            var statementPart = statementSyntax.Parent;
            while (count < 10)
            {
                statementPart = statementPart.Parent;

                if (statementPart is MethodDeclarationSyntax)
                {
                    return (statementPart as MethodDeclarationSyntax).Identifier.ValueText;
                }

                if (statementPart is ConstructorDeclarationSyntax)
                {
                    return (statementPart as ConstructorDeclarationSyntax).Identifier.ValueText;
                }

                if (statementPart is PropertyDeclarationSyntax)
                {
                    return (statementPart as PropertyDeclarationSyntax).Identifier.ValueText;
                }

                count++;
            }

            return "Unkown";
        }
    }
}
