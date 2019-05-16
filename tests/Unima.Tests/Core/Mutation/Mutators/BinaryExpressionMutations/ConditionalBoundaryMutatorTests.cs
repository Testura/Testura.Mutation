using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Unima.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Unima.Tests.Core.Mutation.Mutators.BinaryExpressionMutations
{
    [TestFixture]
    public class ConditionalBoundaryMutatorTests
    {
        [TestCase("<", "<=")]
        [TestCase("<=", "<")]
        [TestCase(">", ">=")]
        [TestCase(">=", ">")]
        public void ConditionalTests(string conditional, string mutatedConditional)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if(i{conditional}1)}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ConditionalBoundaryMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

           Assert.AreEqual($"i {mutatedConditional} 1", doc[0].MutationDetails.Mutation.ToString());
        }

        [TestCase("result.Item1.IsSuccessStatusCode()", "!(result.Item1.IsSuccessStatusCode())")]
        [TestCase("!result.Item1.IsSuccessStatusCode()", "result.Item1.IsSuccessStatusCode()")]
        public void ConditionalExpression(string conditional, string mutatedConditional)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if({conditional})}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ConditionalBoundaryMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual($"{mutatedConditional}", doc[0].MutationDetails.Mutation.ToString());
        }
    }
}
