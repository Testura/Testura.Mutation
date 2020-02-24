using System.Collections.Generic;
using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentFilterItemTests
    {
        [TestCase("KlarnaMock.cs", true, TestName = "IsAllowed_WhenHavingResourceThatMatchWildCardFilter_ShouldReturnTrue")]
        [TestCase("klarnamock.cs", true, TestName = "IsAllowed_WhenHavingResourceWithLowerCaseThatMatchWildCardFilter_ShouldReturnTrue")]
        [TestCase("kfdsf.cs", false, TestName = "IsAllowed_WhenHavingResourceThatDontMatchWildCardFilter_ShouldReturnFalse")]
        [TestCase("/mock/hej.cs", true, TestName = "IsAllowed_WhenHavingResourceWithSlashThatMatchWildCardFilter_ShouldReturnTrue")]
        public void IsAllowed(string resource, bool shouldBeAllowed)
        {
            var mutationDocumentFilterItem = new MutationDocumentFilterItem
            {
                Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                Resource = "*Mock*"
            };

            Assert.AreEqual(shouldBeAllowed, mutationDocumentFilterItem.IsAllowed(resource));
        }

        [TestCase("\\weird\\hej.cs", true, TestName = "IsAllowed_WhenHavingResourceWithMaskedSlashButStillMatchFilter_ShouldReturnTrue")]
        [TestCase("/weird/hej.cs", true, TestName = "IsAllowed_WhenHavingResourceWithCorrectSlashANdMa0tchFilter_ShouldReturnTrue")]
        public void IsAllowedSpecialPath(string resource, bool shouldBeAllowed)
        {
            var mutationDocumentFilterItem = new MutationDocumentFilterItem
            {
                Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                Resource = "/weird/hej.cs"
            };

            Assert.AreEqual(shouldBeAllowed, mutationDocumentFilterItem.IsAllowed(resource));
        }

        [TestCase(200, true, TestName = "MatchFilterLines_WhenHavingLineAt200_ShouldReturnTrue")]
        [TestCase(202, true, TestName = "MatchFilterLines_WhenHavingLineAt202_ShouldReturnTrue")]
        [TestCase(203, false, TestName = "MatchFilterLines_WhenHavingLineAt203_ShouldReturnFalse")]
        public void MatchFilterLines(int line, bool shouldBeAllowed)
        {
            var mutationDocumentFilterItem = new MutationDocumentFilterItem
            {
                Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                Resource = "Test.cs",
                Lines = new List<string> { "200,2" }
            };

            Assert.AreEqual(shouldBeAllowed, mutationDocumentFilterItem.MatchFilterLines(line));
        }
    }
}
