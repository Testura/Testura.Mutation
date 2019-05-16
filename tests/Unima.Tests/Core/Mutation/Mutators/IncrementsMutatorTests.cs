using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Unima.Core.Creator.Mutators;

namespace Unima.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class IncrementsMutatorTests
    {
        [TestCase("i++", "i--", TestName = "GetMutatedDocument_WhenHavingAStatementWithPostIncrementExpression_ShouldMutateIt")]
        [TestCase("i--", "i++", TestName = "GetMutatedDocument_WhenHavingAStatementWithPostIncrementExpression_ShouldMutateIt")]
        public void Positive(string preMutation, string postMutation)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{public void Do(){{{preMutation};}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new IncrementsMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(postMutation, doc[0].MutationDetails.Mutation.ToString());
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
