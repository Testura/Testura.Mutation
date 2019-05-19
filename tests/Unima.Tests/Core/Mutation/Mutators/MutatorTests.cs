using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Unima.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Unima.Tests.Core.Mutation.Mutators
{
    [TestFixture]
    public class MutatorTests
    {
        [Test]
        public void GetMutatedDocument_GetMutatedDocumentWithExcludeFromCodeCoverageOnClass_ShouldNotGetAnyMutations()
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                    [ExcludeFromCodeCoverage]
                    class C { 
                        
                        public int Propi { get{ return 1+2; }} 

                        public void FirstMethod() { var i = 1 + 2; } 
                    
                        public void SecondMethod() { var k = 1 + 2; }
                    }");

            var root = tree.GetRoot();

            var mutator = new MathMutator();
            var documents = mutator.GetMutatedDocument(root, null);
            Assert.IsEmpty(documents);
        }

        [Test]
        public void GetMutatedDocument_GetMutatedDocumentWithExcludeFromCodeCoverageOnSingleMethod_ShouldGetMutationOnOtherMethod()
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                    class C { 
                        
                        public int Propi { get{ return 1+2; }} 
                    
                        public void FirstMethod() { var i = 1 + 2; } 
                    
                        [ExcludeFromCodeCoverage]
                        public void SecondMethod() { var k = 1 + 2; }
                    }");
            var root = tree.GetRoot();

            var mutator = new MathMutator();
            var documents = mutator.GetMutatedDocument(root, null);
            
            Assert.AreEqual(2, documents.Count);
            Assert.IsTrue(documents.Any(d => d.MutationName.Contains("FirstMethod")));
            Assert.IsTrue(documents.Any(d => d.MutationName.Contains("Propi")));
        }

        [Test]
        public void GetMutatedDocument_GetMutatedDocumentWithExcludeFromCodeCoverageOnProperty_ShouldGetMutationOnMethods()
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                    class C { 

                        [ExcludeFromCodeCoverage]
                        public int Propi { get{ return 1+2; }} 
                  
                        public void FirstMethod() { var i = 1 + 2; } 
                    
                        public void SecondMethod() { var k = 1 + 2; }
                    }");
            var root = tree.GetRoot();

            var mutator = new MathMutator();
            var documents = mutator.GetMutatedDocument(root, null);

            Assert.AreEqual(2, documents.Count);
            Assert.IsTrue(documents.Any(d => d.MutationName.Contains("FirstMethod")));
            Assert.IsTrue(documents.Any(d => d.MutationName.Contains("SecondMethod")));
        }

        [Test]
        public void GetMutatedDocument_GetMutatedDocumentWithDifferentAttribute_ShouldGetAllMutations()
        {
            var tree = SyntaxFactory.ParseSyntaxTree(@"
                    [DifferentAttribute]
                    class C { 

                        public int Propi { get{ return 1+2; }} 
                  
                        public void FirstMethod() { var i = 1 + 2; } 
                    
                        public void SecondMethod() { var k = 1 + 2; }
                    }");
            var root = tree.GetRoot();

            var mutator = new MathMutator();
            var documents = mutator.GetMutatedDocument(root, null);

            Assert.AreEqual(3, documents.Count);
        }
    }
}
