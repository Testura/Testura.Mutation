using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public class MethodCallMutator : Mutator
    {
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            Replacers.Add(new MutationDocumentDetails(node, SyntaxFactory.EmptyStatement(), GetWhere(node)));
            return base.VisitExpressionStatement(node);
        }
    }
}
