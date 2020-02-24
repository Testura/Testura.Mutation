using System.Linq;
using System.Threading;
using NUnit.Framework;
using Testura.Mutation.Core.Creator;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Core.Creator
{
    [TestFixture]
    public class MutationDocumentCreatorTests
    {
        private MutationDocumentCreator _mutationDocumentCreator;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mutationDocumentCreator = new MutationDocumentCreator();
        }

        [Test]
        public void CreateMutations_WhenCreatingMutationsAndDontHaveFilter_ShouldGetMutationsForDocuments()
        {
            var config = ConfigCreator.CreateConfig();

            var mutationDocuments = _mutationDocumentCreator.CreateMutations(config);
            Assert.AreEqual(3, mutationDocuments.Count);
        }

        [Test]
        public void CreateMutations_WhenCreatingMutationsAndWeCancel_ShouldGetEmptyMutationList()
        {
            var config = ConfigCreator.CreateConfig();
            var cancellation = new CancellationTokenSource();
            var token = cancellation.Token;
            cancellation.Cancel();

            var mutationDocuments = _mutationDocumentCreator.CreateMutations(config, token);
            Assert.AreEqual(0, mutationDocuments.Count);
        }

        [Test]
        public void CreateMutations_WhenCreatingMutationsAndWeDontHaveAnyDocuments_ShouldGetEmptyMutationList()
        {
            var config = ConfigCreator.CreateConfig();
            config.Solution = config.Solution.Projects.Last().RemoveDocument(config.Solution.Projects.Last().Documents.First().Id).Solution;
            config.Solution = config.Solution.Projects.Last().RemoveDocument(config.Solution.Projects.Last().Documents.First().Id).Solution;

            var mutationDocuments = _mutationDocumentCreator.CreateMutations(config);
            Assert.AreEqual(0, mutationDocuments.Count);
        }
    }
}
