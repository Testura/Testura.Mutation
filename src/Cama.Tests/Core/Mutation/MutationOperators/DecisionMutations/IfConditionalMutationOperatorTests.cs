using Cama.Core.Mutation.MutationOperators.DecisionMutations;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.MutationOperators.DecisionMutations
{
    [TestFixture]
    public class IfConditionalMutationOperatorTests
    {
        [TestCase("==", "!=")]
        [TestCase("!=", "==")]
        [TestCase("<", ">")]
        [TestCase("<=", ">")]
        [TestCase(">", "<")]
        [TestCase(">=", "<")]
        public void ConditionalTests(string conditional, string mutatedConditional)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if(i{conditional}1)}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new IfConditionalMutationOperator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null, null);

           Assert.AreEqual($"if(i{mutatedConditional}1)", doc[0].Replacer.Replace.ToString());
        }
    }
}
