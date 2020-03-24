using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Testura.Mutation.Tests.Core.Mutation.Mutators.BinaryExpressionMutations
{
    [TestFixture]
    public class ConditionalBoundaryMutatorTests
    {
        [TestCase("<", "<=", "LessThanToken")]
        [TestCase("<=", "<", "LessThanEqualsToken")]
        [TestCase(">", ">=", "GreaterThanToken")]
        [TestCase(">=", ">", "GreaterThanEqualsToken")]
        public void ConditionalTests(string conditional, string mutatedConditional, string category)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if(i{conditional}1)}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ConditionalBoundaryMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

           Assert.AreEqual($"i {mutatedConditional} 1", doc[0].MutationDetails.Mutation.ToString());
           Assert.AreEqual(MutationOperators.ConditionalBoundary, doc[0].MutationDetails.Category.HeadCategory);
           Assert.AreEqual(category, doc[0].MutationDetails.Category.Subcategory);
        }

        [TestCase("result.Item1.IsSuccessStatusCode()", "!(result.Item1.IsSuccessStatusCode())", "InvocationExpression")]
        [TestCase("!result.Item1.IsSuccessStatusCode()", "result.Item1.IsSuccessStatusCode()", "LogicalNotExpression")]
        public void ConditionalExpression(string conditional, string mutatedConditional, string category)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if({conditional})}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ConditionalBoundaryMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual($"{mutatedConditional}", doc[0].MutationDetails.Mutation.ToString());
            Assert.AreEqual(MutationOperators.ConditionalBoundary, doc[0].MutationDetails.Category.HeadCategory);
            Assert.AreEqual(category, doc[0].MutationDetails.Category.Subcategory);
        }
    }
}
