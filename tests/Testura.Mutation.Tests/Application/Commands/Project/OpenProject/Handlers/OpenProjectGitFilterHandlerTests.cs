using System;
using System.Threading;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject.Handlers
{
    [TestFixture]
    public class OpenProjectGitFilterHandlerTests
    {
        private OpenProjectGitFilterHandler _openProjectGitFilterHandler;

        [SetUp]
        public void SetUp()
        {
            _openProjectGitFilterHandler = new OpenProjectGitFilterHandler(new MutationDocumentFilterItemGitDiffCreator(new GitDiffStub()));
        }

        [Test]
        public void InitializeGitFilter_WhenInitializeGitFilterAndGitInfoIsNull_ShouldNotAddAnyFilterItems()
        {
            var filterItems = _openProjectGitFilterHandler.CreateGitFilterItems("myPath", null);
            Assert.AreEqual(0, filterItems.Count);
        }

        [Test]
        public void InitializeGitFilter_WhenInitializeGitFilterAndGitInfoIsNotNullButGenerateIsFalse_ShouldNotAddAnyFilterItems()
        {
            var filterItems = _openProjectGitFilterHandler.CreateGitFilterItems("myPath", new GitInfo { GenerateFilterFromDiffWithMaster = false });
            Assert.AreEqual(0, filterItems.Count);
        }

        [Test]
        public void InitializeGitFilter_WhenInitializeGitFilterWithGitInfo_ShouldAddFilterItems()
        {
            var filterItems = _openProjectGitFilterHandler.CreateGitFilterItems("myPath", new GitInfo { GenerateFilterFromDiffWithMaster = true });
            Assert.AreEqual(2, filterItems.Count);
        }

        [Test]
        public void VerifySolutionExist_WhenPathExistButWeCancelWithToken_ShouldCancel()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => _openProjectGitFilterHandler.CreateGitFilterItems("myPath", new GitInfo { GenerateFilterFromDiffWithMaster = true }, token));
        }
    }
}
