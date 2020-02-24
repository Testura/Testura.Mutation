using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentFilterTests
    {
        [Test]
        public void ResourceAllowed_WhenHavingNoFilter_ShouldReturnTrue()
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>();

            Assert.IsTrue( mutationDocumentFilter.ResourceAllowed("test.cs"));
        }

        [TestCase("test.cs", true, TestName = "ResourceAllowed_WhenHavingAResourceThatIsAllowed_ShouldReturnTrue")]
        [TestCase("hej.cs", false, TestName = "ResourceAllowed_WhenHavingAResourceThatIsNotAllowed_ShouldReturnFrue")]
        public void ResourceAllowed(string resource, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "Test.cs" }
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceAllowed(resource));
        }

        [TestCase("test.cs", false, TestName = "ResourceAllowed_WhenWeHaveAResourceButItsOnDeny_ShouldReturnFalse")]
        [TestCase("hej.cs", true, TestName = "ResourceAllowed_WhenWeHaveAResourceButItsNotOnDeny_ShouldReturnTrue")]
        [TestCase("/test/hej.cs", false, TestName = "ResourceAllowed_WhenWeHaveAResourceButItsOnDenyListByWildcard_ShouldReturnFalse")]
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

        [Test]
        public void ResourceLinesAllowed_WhenHavingNoFilter_ShouldReturnTrue()
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>();

            Assert.IsTrue(mutationDocumentFilter.ResourceLinesAllowed("test.cs", 200));
        }

        [Test]
        public void ResourceLinesAllowed_WhenFilterIsNull_ShouldReturnTrue()
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = null;

            Assert.IsTrue(mutationDocumentFilter.ResourceLinesAllowed("test.cs", 200));
        }

        [Test]
        public void ResourceLinesAllowed_WhenHavingNoFilterWithLines_ShouldReturnTrue()
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Resource = "hej.cs", Effect = MutationDocumentFilterItem.FilterEffect.Deny }
            };

            Assert.IsTrue(mutationDocumentFilter.ResourceLinesAllowed("test.cs", 200));
        }


        [TestCase("hej.cs", 200, true, TestName = "ResourceLinesAllowed_WhenHavingSourceThatMatchResourceAndLine_ShouldReturnTrue")]
        [TestCase("hej.cs", 205, false, TestName = "ResourceLinesAllowed_WhenHavingSourceThatMatchResourceButNotLine_ShouldReturnFalse")]
        [TestCase("hej.cs", 300, false, TestName = "ResourceLinesAllowed_WhenHavingSourceThatMatchResourceAndLineButLineIsOnDeny_ShouldReturnFalse")]
        public void AllowLines(string resource, int line, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "hej.cs", Lines = new List<string> { "200" }},
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "hej.cs", Lines = new List<string> { "300" }},
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceLinesAllowed(resource, line));
        }

        [TestCase("hej.cs", 205, true, TestName = "ResourceLinesAllowed_WhenHavingSourceThatMatchResourceAndWeOnlyHaveOneDenyLineThatDoesntMatch_ShouldReturnTrue")]
        [TestCase("hej.cs", 300, false, TestName = "ResourceLinesAllowed_WhenHavingSourceThatMatchResourceAndWeOnlyHaveOneDenyLineThatWeMatch_ShouldReturnFalse")]
        public void AllowLinesWithOnlyDeny(string resource, int line, bool shouldBeAccepted)
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "hej.cs", Lines = new List<string> { "300" }},
            };

            Assert.AreEqual(shouldBeAccepted, mutationDocumentFilter.ResourceLinesAllowed(resource, line));
        }

        [TestCase("LogTo.*", false, TestName = "CodeAllowed_WhenHavingCodeThatMatchConstrainAndItsOnDeny_ShouldReturnFalse")]
        [TestCase("LogTo.", true, TestName = "CodeAllowed_WhenHavingCodeThatNotMatchConstrainAndItsOnDeny_ShouldReturnTrue")]
        [TestCase("LogTo.Info(\"log\")", false, TestName = "CodeAllowed_WhenHavingCodeThatMatchConstrainAndItsOnDeny_ShouldReturnFalse")]
        [TestCase("Woho", true, TestName = "CodeAllowed_WhenHavingCodeThatNotMatchConstrainAndItsOnDeny_ShouldReturnTrue")]
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
        public void CodeAllowed_WhenFilterIsNull_ShouldReturnTrue()
        {
            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = null;

            Assert.AreEqual(true, mutationDocumentFilter.CodeAllowed("Hej.cs", 10, null));
        }

        [Test]
        public void CodeAllowed_WhenHavingFilterItemsButNoOneWithCodeConstrain_ShouldReturnTrue()
        {
            var orginalCode = SyntaxFactory.ParseExpression("LogTo.Info(\"log\")");

            var mutationDocumentFilter = new MutationDocumentFilter();
            mutationDocumentFilter.FilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Deny, Resource = "*"},
                new MutationDocumentFilterItem { Effect = MutationDocumentFilterItem.FilterEffect.Allow, Resource = "Hej.cs", Lines = new List<string> { "10" }},
            };

            Assert.AreEqual(true, mutationDocumentFilter.CodeAllowed("Hej.cs", 10, orginalCode));
        }

        [Test]
        public void CodeAllowed_WhenHavingConstrainWithLines_ShouldReturnTrueIfItMatchContrainThatIsAllowedOnLine()
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
