using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Testura.Mutation.Core.Location;

namespace Testura.Mutation.Core
{
    public class MutationDocumentDetails
    {
        public MutationDocumentDetails(
            SyntaxNode original,
            SyntaxNode mutation,
            MutationLocationInfo location,
            MutationCategory category)
        {
            Original = original;
            Mutation = mutation;
            FullOriginal = GetRoot(original);
            FullMutation = FullOriginal.ReplaceNode(Original, Mutation);
            Location = location;
            Category = category;
        }

        public SyntaxNode Original { get; }

        public SyntaxNode Mutation { get; }

        public CompilationUnitSyntax FullOriginal { get; }

        public CompilationUnitSyntax FullMutation { get; }

        public MutationLocationInfo Location { get; }

        public MutationCategory Category { get; set; }

        private CompilationUnitSyntax GetRoot(SyntaxNode syntaxNode)
        {
            return syntaxNode.FirstAncestorOrSelf<CompilationUnitSyntax>();
        }
    }
}
