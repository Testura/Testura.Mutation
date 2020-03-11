using NUnit.Framework;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Core.Creator.Filter
{
    [TestFixture]
    public class MutationDocumentFilterItemGitDiffCreatorTests
    {
        private MutationDocumentFilterItemGitDiffCreator _mutationDocumentFilterItemGitDiffCreator;

        [SetUp]
        public void SetUp()
        {
            _mutationDocumentFilterItemGitDiffCreator = new MutationDocumentFilterItemGitDiffCreator(new GitDiffStub());
        }

        [Test]
        public void GetFilterItemsFromDiff_WhenGettingFilterFromDiff_ShouldGetFIlter()
        {
            var filter = _mutationDocumentFilterItemGitDiffCreator.GetFilterItemsFromDiff("SomePath");

            Assert.AreEqual(2, filter.Count, "Wrong filter count");

            var filterItem = filter[1];
            Assert.AreEqual("*src/Testura.Mutation.Application/Commands/Project/OpenProject/Handlers/OpenProjectMutatorsHandler.cs", filterItem.Resource, "Wrong resource");
            Assert.AreEqual(MutationDocumentFilterItem.FilterEffect.Allow, filterItem.Effect, "Wrong effect");
            Assert.AreEqual(11, filterItem.Lines.Count, "Wrong filter item lines count");
            Assert.AreEqual("27,2", filterItem.Lines[5], "Wrong for multi line");
            Assert.AreEqual("25", filterItem.Lines[4], "Wrong for single line");
        }
    }
}
