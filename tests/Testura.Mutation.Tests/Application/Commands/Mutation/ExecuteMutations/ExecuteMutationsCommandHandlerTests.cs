using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Mutation.ExecuteMutations;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Loggers;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Application.Commands.Mutation.ExecuteMutations
{
    [TestFixture]
    public class ExecuteMutationsCommandHandlerTests
    {
        private MockFileSystem _fileSystem;
        private MutationConfig _config;
        private TestRunnerDependencyFilesHandler _dependency;
        private MutationDocument _mutationDocument;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig(_fileSystem);
            _dependency = new TestRunnerDependencyFilesHandler(_fileSystem);
            _mutationDocument = new MutationDocumentCreator().CreateMutations(_config).First();
        }

        [Test]
        public async Task Handle_WhenExecuteMutationAndTestFails_ShouldGetCorrectResults()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreateNegative();

            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(new MutationDocumentExecutor(compiler, _dependency, testRunner, _fileSystem), new MutationRunLoggerFactory());
            var results = await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, new List<MutationDocument> { _mutationDocument }), CancellationToken.None);

            Assert.AreEqual(1, results.Count);

            var result = results.First();
            Assert.IsFalse(result.Survived);
        }


        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldLogMutations()
        {
            var mutationRunLoggerFactory = new Mock<IMutationRunLoggerFactory>();

            var mutationRunLogger = new Mock<IMutationRunLogger>();
            mutationRunLoggerFactory.Setup(m => m.GetMutationRunLogger(MutationRunLogger.Azure)).Returns(mutationRunLogger.Object);

            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreateNegative();
            _config.MutationRunLoggers = new List<MutationRunLogger> { MutationRunLogger.Azure };

            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(new MutationDocumentExecutor(compiler, _dependency, testRunner, _fileSystem), mutationRunLoggerFactory.Object);
            await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, new List<MutationDocument> { _mutationDocument }), CancellationToken.None);

            mutationRunLogger.Verify(m => m.LogAfterMutation(It.IsAny<MutationDocument>(), It.IsAny<List<MutationDocumentResult>>(), It.IsAny<int>()), Times.Once);
            mutationRunLogger.Verify(m => m.LogBeforeMutation(It.IsAny<MutationDocument>()), Times.Once);
            mutationRunLogger.Verify(m => m.LogBeforeRun(It.IsAny<List<MutationDocument>>()), Times.Once);
        }
    }
}
