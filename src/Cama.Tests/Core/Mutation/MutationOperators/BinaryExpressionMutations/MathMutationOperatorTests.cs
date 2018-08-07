using Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cama.Tests.Core.Mutation.MutationOperators.BinaryExpressionMutations
{
    [TestFixture]
    public class MathMutationOperatorTests
    {
        [TestCase("1+2", "1-2")]
        [TestCase("1-2", "1+2")]
        [TestCase("1*2", "1/2")]
        [TestCase("1/2", "1*2")]
        [TestCase("1%2", "1*2")]
        [TestCase("1&2", "1|2")]
        [TestCase("1|2", "1&2")]
        [TestCase("1^2", "1&2")]
        [TestCase("1<<2", "1>>2")]
        [TestCase("1>>2", "1<<2")]
        public void BinaryTests(string binary, string mutatedBinary)
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{var i = {binary};}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutationOperator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null, null);

            Assert.AreEqual($"var i = {mutatedBinary};", doc[0].Replacer.Replace.ToString());
        }

        [Test]
        public void BinaryTestMultiple()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{publicvoidDo(){{var i = 1+2/3;}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutationOperator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null, null);

            Assert.AreEqual(2, doc.Count);
            Assert.AreEqual($"var i = 1-2/3;", doc[0].Replacer.Replace.ToString());
            Assert.AreEqual($"var i = 1+2*3;", doc[1].Replacer.Replace.ToString());
        }

        [Test]
        public void BinaryTestFromProperty()
        {
            var tree = SyntaxFactory.ParseSyntaxTree($"classC{{public int Propi{{ get{{ return 1+2; }}}}");
            var root = tree.GetRoot();

            var binaryExpressionMutationOperator = new MathMutationOperator();
            var doc = binaryExpressionMutationOperator.GetMutatedDocument(root, null, null);

            Assert.AreEqual($"return 1-2;", doc[0].Replacer.Replace.ToString());
        }
    }
}
