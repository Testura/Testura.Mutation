using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.Mutators.BinaryExpressionMutations
{
    [TestFixture]
    public class NegateConditionalMutatorTests
    {
        [TestCase("==", "!= ")]
        [TestCase("!=", "== ")]
        [TestCase("<", "> ")]
        [TestCase("<=", "> ")]
        [TestCase(">", "< ")]
        [TestCase(">=", "< ")]
        public void ConditionalTests(string preMutation, string postMutation)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if(i{preMutation}1)}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new NegateConditionalMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null, null);

           Assert.AreEqual($"i{postMutation}1", doc[0].MutationInfo.Mutation.ToString());
        }
    }
}
