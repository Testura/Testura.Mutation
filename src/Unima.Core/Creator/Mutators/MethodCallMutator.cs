using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Unima.Core.Creator.Mutators
{
    public class MethodCallMutator : Mutator
    {
        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var o = node.ToFullString();

            return base.VisitExpressionStatement(node);
        }
    }
}
