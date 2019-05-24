using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Unima.Core.Creator.Mutators;

namespace Unima.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class MethodCallMutatorTests
    {
        [Test]
        public void GetMutatedDocument_WhenCallingOnAMethodWithoutReturnValue_ShouldReplaceCallWithComment()
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                    class C { 
                        public void FirstMethod() { Hello(); } 
                    }");
            var root = tree.GetRoot();

            var mutator = new MethodCallMutator();
            var doc = mutator.GetMutatedDocument(root, null);

            Assert.AreEqual("/* Removed this method call */", doc[0].MutationDetails.Mutation.ToString());
        }
    }
}
