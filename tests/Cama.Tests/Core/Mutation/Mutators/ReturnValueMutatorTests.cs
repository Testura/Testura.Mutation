using Cama.Core.Creator.Mutators;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class ReturnValueMutatorTests
    {
        [TestCase("true", "return false;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnTrue_ShouldReturnFalse")]
        [TestCase("false", "return true;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnTrue_ShouldReturnTrue")]
        [TestCase("1", "return 0;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturn1_ShouldReturn0")]
        [TestCase("0", "return 1;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturn0_ShouldReturn1")]
        [TestCase("30.2", "return 0;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnBiggerNumber_ShouldReturn0")]
        [TestCase("new Obj()", "return null;", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnNewObject_ShouldReturnNull")]
        [TestCase("null", "throw new Exception(\"Mmmmutation\");", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnNull_ShouldThrowException")]
        [TestCase("\"test\"", "return \"Mutation\";", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnAString_ShouldNewString")]
        public void Positive(string preMutation, string postMutation)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{return {preMutation};}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ReturnValueMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(postMutation, doc[0].MutationDetails.Mutation.ToString());
        }

        [Test]
        public void GetMutationDocument_WhenHavingReturnThatCallOnDifferentMethod_ShouldNotMutate()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{return it.Do(true);}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ReturnValueMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.IsEmpty(doc);
        }
    }
}
