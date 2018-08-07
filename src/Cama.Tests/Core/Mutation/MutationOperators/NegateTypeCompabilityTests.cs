using Cama.Core.Mutation.MutationOperators;
using Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Cama.Tests.Core.Mutation.MutationOperators
{
    [TestFixture]
    public class NegateTypeCompabilityTests
    {

        [TestCase("i is bool", "!(i is bool)")]
        [TestCase("i is bool && o == 1", "!(i is bool )&& o == 1")]
        public void Test(string conditional, string mutatedConditional)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if({conditional})}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new NegateTypeCompabilityMutationOperator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null, null);

            Assert.AreEqual($"if({mutatedConditional})", doc[0].Replacer.Replace.ToString());
        }
    }
}
