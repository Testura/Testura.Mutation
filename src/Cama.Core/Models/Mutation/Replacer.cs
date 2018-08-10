using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Models.Mutation
{
    public class Replacer
    {
        public Replacer(SyntaxNode orginal, SyntaxNode mutation, string where)
        {
            Orginal = orginal;
            Mutation = mutation;
            FullOrginal = GetRoot(orginal);
            FullMutation = FullOrginal.ReplaceNode(Orginal, Mutation);
            Where = where;
        }

        public SyntaxNode Orginal { get; }

        public SyntaxNode Mutation { get; }

        public CompilationUnitSyntax FullOrginal { get; }

        public CompilationUnitSyntax FullMutation { get; }

        public string Where { get; }

        private CompilationUnitSyntax GetRoot(SyntaxNode syntaxNode)
        {
            return syntaxNode.FirstAncestorOrSelf<CompilationUnitSyntax>();
        }
    }
}
