using System.Reflection.Emit;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Testura.Mutation.Tests.Core.Mutation.Mutators.BinaryExpressionMutations
{
    [TestFixture]
    public class MathMutatorTests
    {
        [TestCase("1+2", "1 - 2", "PlusToken")]
        [TestCase("1-2", "1 + 2", "MinusToken")]
        [TestCase("1*2", "1 / 2", "AsteriskToken")]
        [TestCase("1/2", "1 * 2", "SlashToken")]
        [TestCase("1%2", "1 * 2", "PercentToken")]
        [TestCase("1&2", "1 | 2", "AmpersandToken")]
        [TestCase("1|2", "1 & 2", "BarToken")]
        [TestCase("1^2", "1 & 2", "CaretToken")]
        [TestCase("1<<2", "1 >> 2", "LessThanLessThanToken")]
        [TestCase("1>>2", "1 << 2", "GreaterThanGreaterThanToken")]
        public void BinaryTests(string binary, string mutatedBinary, string category)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{var i = {binary};}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(mutatedBinary, doc[0].MutationDetails.Mutation.ToString());
            Assert.AreEqual(MutationOperators.Math.ToString(), doc[0].MutationDetails.Category.HeadCategory);
            Assert.AreEqual(category, doc[0].MutationDetails.Category.Subcategory);
        }

        [Test]
        public void GetMutatedDocument_WhenHavingBinaryExpressionThatContainsAString_ShouldNotMutate()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{var i = \"test\" + 2; }}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.IsEmpty(doc, "Should not be able to find any mutations");
        }

        [Test]
        public void GetMutatedDocument_WhenHavingMultipleMathOperationsInSameStatement_ShouldMutateAll()
        {
            var tree = SyntaxFactory.ParseSyntaxTree("classC{{publicvoidDo(){{var i = 1+2/3;}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual(2, doc.Count);
            Assert.AreEqual("1 - 2 / 3", doc[0].MutationDetails.Mutation.ToString());
            Assert.AreEqual("2 * 3", doc[1].MutationDetails.Mutation.ToString());
        }

        [Test]
        public void GetMutatedDocument_WhenHavingAReturnStatementWithMathOperator_ShouldMutate()
        {
            var tree = SyntaxFactory.ParseSyntaxTree("classC{{public int Propi{{ get{{ return 1+2; }}}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual("1 - 2", doc[0].MutationDetails.Mutation.ToString());
        }

        [Test]
        public void GetMutatedDocument_WhenHavingABinaryExpressionWithoutStatement_ShouldMutate()
        {
            var tree = SyntaxFactory.ParseSyntaxTree("classC{{public int Propi => 1 + 2");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null);

            Assert.AreEqual("1 - 2", doc[0].MutationDetails.Mutation.ToString());
        }
    }
}
