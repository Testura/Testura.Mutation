using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Models.Mutation
{
    public class Replacer
    {
        public StatementSyntax Orginal { get; set; }

        public StatementSyntax Replace { get; set; }

        public MethodDeclarationSyntax Method { get; set; }
    }
}
