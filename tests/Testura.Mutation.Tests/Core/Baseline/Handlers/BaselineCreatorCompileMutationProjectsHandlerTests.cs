using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Core.Baseline.Handlers
{
    [TestFixture]
    public class BaselineCreatorCompileMutationProjectsHandlerTests
    {
        private const string Path = "test/path";

        private MockFileSystem _fileSystem;
        private MutationConfig _config;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig();
        }

        [Test]
        public void CompileMutationProjects_WhenCompilerReturnsError_ShouldThrowException()
        {
            var compiler = ProjectCompilerCreator.CreateNegativeCompiler(_fileSystem);
            var baselineCreatorCompileMutationProjectsHandler = new BaselineCreatorCompileMutationProjectsHandler(compiler, _fileSystem);

            var exception = Assert.ThrowsAsync<BaselineException>(async () => await baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(_config, Path));
            var compilationException = exception.InnerException as CompilationException;

            Assert.AreEqual(1, compilationException.ErrorMessages.Count);
            Assert.AreEqual("Compile message", compilationException.ErrorMessages.First());
        }

        [Test]
        public async Task CompileMutationProjects_WhenCompilerNotReturnsError_ShouldNotThrowExceptionAndShouldHaveCreatedABaselineDirectory()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var baselineCreatorCompileMutationProjectsHandler = new BaselineCreatorCompileMutationProjectsHandler(compiler, _fileSystem);

            await baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(_config, Path);

            Assert.IsTrue(_fileSystem.Directory.Exists(Path), "Have not created the baseline directory");
        }

        [Test]
        public void CompileMutationProjects_WhenSolutionDoesntHaveProjectWithName_ShouldThrowException()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var baselineCreatorCompileMutationProjectsHandler = new BaselineCreatorCompileMutationProjectsHandler(compiler, _fileSystem);

            _config.MutationProjects.First().Project.Name = "WAAA";

            var exception = Assert.ThrowsAsync<BaselineException>(async () => await baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(_config, Path));
            StringAssert.Contains("Could not find any project with the name WAAA", exception.Message);
        }

        [Test]
        public async Task CompileMutationProjects_WhenCancel_ShouldThrowCancellException()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var baselineCreatorCompileMutationProjectsHandler = new BaselineCreatorCompileMutationProjectsHandler(compiler, _fileSystem);

            var cts = new CancellationTokenSource();
            var token = cts.Token;

            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(_config, Path, token));
        }
    }
}
