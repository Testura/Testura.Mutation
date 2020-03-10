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
        private MutationDocument _mutationDocument;
        private MutationDocumentExecutor _mutationExecutor;

        [SetUp]
        public void SetUp()
        {
            var fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig(fileSystem);
            _mutationDocument = new MutationDocumentCreator().CreateMutations(_config).First();

            var dependency = new TestRunnerDependencyFilesHandler(fileSystem);

            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(fileSystem);
            var testRunner = TestRunnerClientCreator.CreateNegative();

            _mutationExecutor = new MutationDocumentExecutor(compiler, dependency, testRunner, fileSystem);

        }

        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldGetCorrectResults()
        {
            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, new MutationRunLoggerManager());
            var results = await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, new List<MutationDocument> { _mutationDocument }), CancellationToken.None);

            Assert.AreEqual(1, results.MutationDocumentResults.Count);

            var result = results.MutationDocumentResults.First();
            Assert.IsFalse(result.Survived);
        }

        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldGetCallBacks()
        {
            var startedCallbackCount = 0;
            var completedCallbackCount = 0;

            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, new MutationRunLoggerManager());

            await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, new List<MutationDocument> { _mutationDocument }, document => startedCallbackCount++, documentResult => completedCallbackCount++), CancellationToken.None);

            Assert.AreEqual(1, startedCallbackCount, "wrong started callback count");
            Assert.AreEqual(1, completedCallbackCount, "Wrong completed callback count");
        }


        [Test]
        public async Task Handle_WhenExecuteMutation_ShouldLogMutations()
        {
            var mutationRunLoggerManager = new Mock<IMutationRunLoggerManager>();
            var mutationDocumentExecutor = new ExecuteMutationsCommandHandler(_mutationExecutor, mutationRunLoggerManager.Object);

            await mutationDocumentExecutor.Handle(new ExecuteMutationsCommand(_config, new List<MutationDocument> { _mutationDocument }), CancellationToken.None);

            mutationRunLoggerManager.Verify(m => m.LogAfterMutation(It.IsAny<MutationDocument>(), It.IsAny<List<MutationDocumentResult>>(), It.IsAny<int>()), Times.Once);
            mutationRunLoggerManager.Verify(m => m.LogBeforeMutation(It.IsAny<MutationDocument>()), Times.Once);
            mutationRunLoggerManager.Verify(m => m.LogBeforeRun(It.IsAny<List<MutationDocument>>()), Times.Once);
        }
    }
}
