using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.Mutators.BinaryExpressionMutations
{
    [TestFixture]
    public class ConditionalBoundaryMutatorTests
    {
        [TestCase("<", "<= ")]
        [TestCase("<=", "< ")]
        [TestCase(">", ">= ")]
        [TestCase(">=", "> ")]
        public void ConditionalTests(string conditional, string mutatedConditional)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if(i{conditional}1)}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ConditionalBoundaryMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null, null);

           Assert.AreEqual($"if(i{mutatedConditional}1)", doc[0].Replacer.Mutation.ToString());
        }
    }
}
