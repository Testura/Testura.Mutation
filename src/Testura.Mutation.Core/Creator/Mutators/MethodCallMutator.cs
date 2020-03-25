using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Testura.Mutation.Core.Creator.Mutators
{
    public class MethodCallMutator : Mutator
    {
        protected override MutationOperators Category => MutationOperators.MethodCall;

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            Replacers.Add(new MutationDocumentDetails(
                node,
                node.WithLeadingTrivia(TriviaList(Comment("/*"))).WithTrailingTrivia(TriviaList(Comment("*/"))),
                GetWhere(node),
                CreateCategory("InvocationExpression")));

            return base.VisitExpressionStatement(node);
        }
    }
}
