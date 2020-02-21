using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentFilterTests
    {
        [TestCase("test.cs", true)]
        [TestCase("hej.cs", false)]
        public void Allow(string resource, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "Test.cs" }
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceAllowed(resource));
        }

        [TestCase("test.cs", false)]
        [TestCase("hej.cs", true)]
        [TestCase("/test/hej.cs", false)]
        public void AllowWithIgnore(string resource, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "*" },
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "Test.cs" },
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "*/test/*" }
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceAllowed(resource));
        }


        [TestCase("hej.cs", 200, true)]
        [TestCase("hej.cs", 205, false)]
        [TestCase("hej.cs", 300, false)]
        public void AllowLines(string resource, int line, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "hej.cs", Lines = new List<string> { "200" }},
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "hej.cs", Lines = new List<string> { "300" }},
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceLinesAllowed(resource, line, null));
        }

        [TestCase("hej.cs", 205, true)]
        [TestCase("hej.cs", 300, false)]
        public void AllowLinesWithOnlyDeny(string resource, int line, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "hej.cs", Lines = new List<string> { "300" }},
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceLinesAllowed(resource, line, null));
        }

        [TestCase("LogTo.*", false)]
        [TestCase("LogTo.", true)]
        [TestCase("LogTo.Info(\"log\")", false)]
        [TestCase("Woho", true)]
        public void CodeConstrain(string code, bool expectedValue)
        {
            var orginalCode = SyntaxFactory.ParseExpression("LogTo.Info(\"log\")");

            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "*", CodeConstrain = code},
            };

            Assert.AreEqual(expectedValue, mutationDocumentFilter.CodeAllowed("Hej.cs", 10, orginalCode));
        }

        [Test]
        public void CodeConstrainWithLines()
        {
            var orginalCode = SyntaxFactory.ParseExpression("LogTo.Info(\"log\")");

            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "*", CodeConstrain = "LogTo.*"},
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "Hej.cs", Lines = new List<string> { "10" }, CodeConstrain = "LogTo.*"},
            };

            Assert.AreEqual(true, mutationDocumentFilter.CodeAllowed("Hej.cs", 10, orginalCode));
        }
    }
}
