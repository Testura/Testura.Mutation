using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Models.Mutation
{
    public class Replacer
    {
        public Replacer(StatementSyntax orginal, StatementSyntax mutation, string where)
        {
            Orginal = orginal;
            Mutation = mutation;
            FullOrginal = GetRoot(orginal);
            FullMutation = FullOrginal.ReplaceNode(Orginal, Mutation);
            Where = where;
        }

        public StatementSyntax Orginal { get; }

        public StatementSyntax Mutation { get; }

        public CompilationUnitSyntax FullOrginal { get; }

        public CompilationUnitSyntax FullMutation { get; }

        public string Where { get; }

        private CompilationUnitSyntax GetRoot(CSharpSyntaxNode syntaxNode)
        {
            return syntaxNode.FirstAncestorOrSelf<CompilationUnitSyntax>();
        }
    }
}
