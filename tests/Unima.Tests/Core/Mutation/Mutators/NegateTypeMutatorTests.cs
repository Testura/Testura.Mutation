using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Unima.Core.Creator.Mutators;

namespace Unima.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class NegateTypeMutatorTests
    {

        [TestCase("i is bool", "!(i is bool)", TestName = "GetMutatedDocument_WhenHavingATypeCompabilityCheck_ShouldMutateIt")]
        [TestCase("i is bool && o == 1", "!(i is bool)", TestName = "GetMutatedDocument_WhenHavingATypeCompabilityCheckInsideAComplexStatement_ShouldMutateIt")]
        public void Positive(string preMutation, string postMutation)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{if({preMutation})}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new NegateTypeCompabilityMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(postMutation, doc[0].MutationDetails.Mutation.ToString());
        }
    }
}
