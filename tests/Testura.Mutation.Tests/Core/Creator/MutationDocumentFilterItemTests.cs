using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentFilterItemTests
    {
        [TestCase("KlarnaMock.cs", true)]
        [TestCase("klarnamock.cs", true)]
        [TestCase("kfdsf.cs", false)]
        [TestCase("/mock/hej.cs", true)]
        public void IsAllowed(string resource, bool shouldBeAllowed)
        {
            var mutationDocumentFilterItem = new MutationDocumentFilterItem
            {
                Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                Resource = "*Mock*"
            };

            Assert.AreEqual(shouldBeAllowed, mutationDocumentFilterItem.IsAllowed(resource));
        }

        [TestCase("\\weird\\hej.cs", true)]
        [TestCase("/weird/hej.cs", true)]
        public void IsAllowedSpecialPath(string resource, bool shouldBeAllowed)
        {
            var mutationDocumentFilterItem = new MutationDocumentFilterItem
            {
                Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                Resource = "/weird/hej.cs"
            };

            Assert.AreEqual(shouldBeAllowed, mutationDocumentFilterItem.IsAllowed(resource));
        }
    }
}
