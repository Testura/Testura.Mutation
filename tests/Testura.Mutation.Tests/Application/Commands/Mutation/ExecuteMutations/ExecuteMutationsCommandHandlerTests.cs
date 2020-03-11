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
using Times = Moq.Times;

namespace Testura.Mutation.Tests.Application.Commands.Mutation.ExecuteMutations
{
    [TestFixture]
    public class ExecuteMutationsCommandHandlerTests
    {
        private MutationConfig _config;
        private MutationDocumentExecutor _mutationExecutor;
        private MockFileSystem _fileSystem;
        private IList<MutationDocument> _mutationDocuments;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig(_fileSystem);
            _mutationDocuments = new MutationDocumentCreator().CreateMutations(_config);

            var dependency = new TestRunnerDependencyFilesHandler(_fileSystem);

            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreateNegative();

            _mutationExecutor = new MutationDocumentExecutor(compiler, dependency, testRunner, _fileSystem);
        }

        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldGetCorrectResults()
        {
            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, new MutationRunLoggerManager());
            var results = await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, _mutationDocuments), CancellationToken.None);

            Assert.AreEqual(3, results.MutationDocumentResults.Count);

            var result = results.MutationDocumentResults.First();
            Assert.IsFalse(result.Survived);
        }

        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldGetCallBacks()
        {
            var startedCallbackCount = 0;
            var completedCallbackCount = 0;

            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, new MutationRunLoggerManager());

            await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, _mutationDocuments, document => startedCallbackCount++, documentResult => completedCallbackCount++), CancellationToken.None);

            Assert.AreEqual(3, startedCallbackCount, "wrong started callback count");
            Assert.AreEqual(3, completedCallbackCount, "Wrong completed callback count");
        }


        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldLogMutations()
        {
            var mutationRunLoggerManager = new Mock<IMutationRunLoggerManager>();
            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, mutationRunLoggerManager.Object);

            await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, _mutationDocuments), CancellationToken.None);

            mutationRunLoggerManager.Verify(m => m.LogAfterMutation(It.IsAny<MutationDocument>(), It.IsAny<List<MutationDocumentResult>>(), It.IsAny<int>()), Times.Exactly(3));
            mutationRunLoggerManager.Verify(m => m.LogBeforeMutation(It.IsAny<MutationDocument>()), Times.Exactly(3));
            mutationRunLoggerManager.Verify(m => m.LogBeforeRun(It.IsAny<List<MutationDocument>>()), Times.Once);
        }

        [Test]
        public async Task Handle_WhenExecuteMutationAndGetUnknownError_ShouldFindUnknownMutation()
        {
            var dependency = new TestRunnerDependencyFilesHandler(_fileSystem);

            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreateEmptyTestResult();

            _mutationExecutor = new MutationDocumentExecutor(compiler, dependency, testRunner, _fileSystem);
            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, new MutationRunLoggerManager());


            var results = await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, _mutationDocuments), CancellationToken.None);

            Assert.AreEqual(3, results.MutationDocumentResults.Count);

            var result = results.MutationDocumentResults.First();
            Assert.IsNotNull(result.UnexpectedError);
        }
    }
}
