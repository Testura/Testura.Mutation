using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using Moq;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject.Handlers
{
    [TestFixture]
    public class OpenProjectSolutionExistHandlerTests
    {
        private OpenProjectSolutionExistHandler _openProjectSolutionExistHandler;
        private MockFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.File.AppendAllText("myFile.sln", "test");

            _openProjectSolutionExistHandler = new OpenProjectSolutionExistHandler(_fileSystem);
        }

        [Test]
        public void VerifySolutionExist_WhenPathExist_ShouldNotThrowException()
        {
            _openProjectSolutionExistHandler.VerifySolutionExist("myFile.sln");
        }

        [Test]
        public void VerifySolutionExist_WhenPathExistButWeCancelWithToken_ShouldCancel()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => _openProjectSolutionExistHandler.VerifySolutionExist("myFile.sln", token));
        }

        [Test]
        public void VerifySolutionExist_WhenPathDontExist_ShouldThrowException()
        {
           Assert.Throws<OpenProjectException>(() => _openProjectSolutionExistHandler.VerifySolutionExist("myFile2.sln"));
        }
    }
}
