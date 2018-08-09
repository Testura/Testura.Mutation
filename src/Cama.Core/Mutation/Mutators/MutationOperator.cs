using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models.Mutation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation.Mutators
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

        protected StatementSyntax GetStatement(ExpressionSyntax binaryExpressionSyntax)
        {
            return binaryExpressionSyntax.FirstAncestorOrSelf<StatementSyntax>();
        }

        protected string GetWhere(CSharpSyntaxNode syntaxNode)
        {
            var methodDeclaration = syntaxNode.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration != null)
            {
                return $"{methodDeclaration.Identifier.Value}(M)";
            }

            var constructorDeclaration = syntaxNode.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (constructorDeclaration != null)
            {
                return $"{constructorDeclaration.Identifier.Value}(C)";
            }

            var propertyDeclaration = syntaxNode.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (propertyDeclaration != null)
            {
                return $"{constructorDeclaration.Identifier.Value}(P)";
            }

            return "Unknown";
        }
    }
}
