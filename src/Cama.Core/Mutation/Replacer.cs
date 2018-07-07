using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cama.Core.Mutation
{
    public class Replacer
    {
        public IfStatementSyntax Orginal { get; set; }

        public IfStatementSyntax Replace { get; set; }

        public MethodDeclarationSyntax Method { get; set; }
    }
}
