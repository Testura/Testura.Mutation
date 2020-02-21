using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentFilterCodeItemTests
    {
        [TestCase("LogTo.*", false)]
        [TestCase("LogTo.", true)]
        [TestCase("LogTo.Info(\"log\")", false)]
        public void IsAllowed(string code, bool expectedValue)
        {
            var orginalCode = SyntaxFactory.ParseExpression("LogTo.Info(\"log\")");
            Assert.AreEqual(expectedValue, new MutationDocumentFilterCodeItem { Code = code }.CodeAreAllowed(orginalCode));
        }
    }
}
