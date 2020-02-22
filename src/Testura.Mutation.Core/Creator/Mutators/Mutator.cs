using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Testura.Mutation.Core.Location;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public abstract class Mutator : CSharpSyntaxRewriter, IMutator
    {
        private const string ExcludeFromCoverageAttributeName = "ExcludeFromCodeCoverage";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Mutator));

        protected Mutator()
        {
            Replacers = new List<MutationDocumentDetails>();
        }

        protected IList<MutationDocumentDetails> Replacers { get; }

        public IList<MutationDocument> GetMutatedDocument(SyntaxNode root, Document document)
        {
            Replacers.Clear();
            Visit(root);

            return Replacers
                .Where(r => !IsExcludedFromCodeCoverage(r.Location, r.Orginal))
                .Select(r => new MutationDocument(document, r)).ToList();
        }

        protected MutationLocationInfo GetWhere(CSharpSyntaxNode syntaxNode)
        {
            var where = "Unknown";

            var methodDeclaration = syntaxNode.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (methodDeclaration != null)
            {
                where = $"{methodDeclaration.Identifier.Value}(M)";
            }

            var constructorDeclaration = syntaxNode.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();

            if (constructorDeclaration != null)
            {
                where = $"{constructorDeclaration.Identifier.Value}(C)";
            }

            var propertyDeclaration = syntaxNode.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            if (propertyDeclaration != null)
            {
                where = $"{propertyDeclaration.Identifier.Value}(P)";
            }

            var location = syntaxNode.GetLocation();
            var locationString = "Unknown line";
            var pos = location.GetLineSpan();
            if (pos.Path != null)
            {
                locationString = $"@({pos.StartLinePosition.Line + 1}:{pos.StartLinePosition.Character + 1})";
            }

            return new MutationLocationInfo { Where = where, Line = locationString };
        }

        protected bool IsExcludedFromCodeCoverage(MutationLocationInfo location, SyntaxNode syntaxNode)
        {
            var attributes = new List<string>();

            var methodDeclaration = syntaxNode.FirstAncestorOrSelf<MethodDeclarationSyntax>();
            var constructorDeclarationSyntax = syntaxNode.FirstAncestorOrSelf<ConstructorDeclarationSyntax>();
            var propertyDeclaration = syntaxNode.FirstAncestorOrSelf<PropertyDeclarationSyntax>();
            var classDeclaration = syntaxNode.FirstAncestorOrSelf<ClassDeclarationSyntax>();

            attributes.AddRange(GetAttributeName(methodDeclaration?.AttributeLists));
            attributes.AddRange(GetAttributeName(constructorDeclarationSyntax?.AttributeLists));
            attributes.AddRange(GetAttributeName(propertyDeclaration?.AttributeLists));
            attributes.AddRange(GetAttributeName(classDeclaration?.AttributeLists));

            if (attributes.Any(a => a.Equals(ExcludeFromCoverageAttributeName, StringComparison.InvariantCultureIgnoreCase)))
            {
                Log.Info($"Ignoring mutation at {location} because of {ExcludeFromCoverageAttributeName} attribute");
                return true;
            }

            return false;
        }

        private IEnumerable<string> GetAttributeName(SyntaxList<AttributeListSyntax>? attributeLists)
        {
            if (attributeLists == null)
            {
                return Enumerable.Empty<string>();
            }

            return attributeLists.Value.SelectMany(al => al.Attributes.Select(a => a.Name.ToString()));
        }
    }
}
