using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators;

namespace Testura.Mutation.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class ReturnValueMutatorTests
    {
        [TestCase("true", "return false;", "TrueLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnTrue_ShouldReturnFalse")]
        [TestCase("false", "return true;", "FalseLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnTrue_ShouldReturnTrue")]
        [TestCase("1", "return 0;", "NumericLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturn1_ShouldReturn0")]
        [TestCase("0", "return 1;", "NumericLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturn0_ShouldReturn1")]
        [TestCase("30.2", "return 0;", "NumericLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnBiggerNumber_ShouldReturn0")]
        [TestCase("new Obj()", "return default(Obj);", "ObjectCreationExpression",  TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnNewObject_ShouldReturnDefault")]
        [TestCase("null", "throw new System.Exception(\"Mmmmutation\");", "NullLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnNull_ShouldThrowException")]
        [TestCase("\"test\"", "return \"Mutation\";", "StringLiteralExpression", TestName = "GetMutatedDocument_WhenHavingAMethodThatReturnAString_ShouldNewString")]
        public void Positive(string preMutation, string postMutation, string subCategory)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{return {preMutation};}}");
            var root = tree.GetRoot();

            var ifConditionalMutationOperator = new ReturnValueMutator();
            var doc = ifConditionalMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(postMutation, doc[0].MutationDetails.Mutation.ToString());
            Assert.AreEqual(MutationOperators.ReturnValue.ToString(), doc[0].MutationDetails.Category.HeadCategory);
            Assert.AreEqual(subCategory, doc[0].MutationDetails.Category.Subcategory);
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
