using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Tests.Utils.Creators;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject.Handlers
{
    [TestFixture]
    public class OpenProjectSolutionExistHandlerTests
    {
        private OpenProjectSolutionHandler _openProjectSolutionExistHandler;
        private MockFileSystem _fileSystem;
        private MutationConfig _config;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.File.AppendAllText("myFile.sln", "test");
            _config = ConfigCreator.CreateConfig(_fileSystem);

            var solutionOpener = new SolutionOpenerStub(_config.Solution);
            _openProjectSolutionExistHandler = new OpenProjectSolutionHandler(_fileSystem, solutionOpener);
        }

        [Test]
        public async Task VerifySolutionExist_WhenPathExist_ShouldNotThrowException()
        {
            var solution = await _openProjectSolutionExistHandler.OpenSolutionAsync("myFile.sln", "debug");
            Assert.AreEqual(_config.Solution, solution);
        }

        [Test]
        public void VerifySolutionExist_WhenPathExistButWeCancelWithToken_ShouldCancel()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await _openProjectSolutionExistHandler.OpenSolutionAsync("myFile.sln", "debug", token));
        }

        [Test]
        public void VerifySolutionExist_WhenPathDontExist_ShouldThrowException()
        {
           Assert.ThrowsAsync<OpenProjectException>(async () => await _openProjectSolutionExistHandler.OpenSolutionAsync("myFile2.sln", "debug"));
        }
    }
}
