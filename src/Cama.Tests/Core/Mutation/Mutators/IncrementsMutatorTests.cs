using Cama.Core.Mutation.Mutators;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.Mutators
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
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null, null);

            Assert.AreEqual(postMutation, doc[0].MutationInfo.Mutation.ToString());
        }

        [Test]
        public void GetMutatedDocument_WhenHavingAFoorLoopWithPostIncrementExpression_ShouldNotMutateIt()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{for(int n = 0; n < 10; n++) {{ }}}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new IncrementsMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null, null);

            Assert.IsEmpty(doc);
        }
    }
}
