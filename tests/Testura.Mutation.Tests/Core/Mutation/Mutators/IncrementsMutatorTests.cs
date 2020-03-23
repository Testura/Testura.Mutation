using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators;

namespace Testura.Mutation.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class IncrementsMutatorTests
    {
        [TestCase("i++", "i--", "PostIncrementExpression", TestName = "GetMutatedDocument_WhenHavingAStatementWithPostIncrementExpression_ShouldMutateIt")]
        [TestCase("i--", "i++", "PostDecrementExpression", TestName = "GetMutatedDocument_WhenHavingAStatementWithPostIncrementExpression_ShouldMutateIt")]
        public void Positive(string preMutation, string postMutation, string category)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{public void Do(){{{preMutation};}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new IncrementsMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(postMutation, doc[0].MutationDetails.Mutation.ToString());
            Assert.AreEqual(MutationOperators.Increment, doc[0].MutationDetails.Category.Category);
            Assert.AreEqual(category, doc[0].MutationDetails.Category.Subcategory);
        }

        [Test]
        public void GetMutatedDocument_WhenHavingAFoorLoopWithPostIncrementExpression_ShouldNotMutateIt()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{for(int n = 0; n < 10; n++) {{ }}}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new IncrementsMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.IsEmpty(doc);
        }
    }
}
