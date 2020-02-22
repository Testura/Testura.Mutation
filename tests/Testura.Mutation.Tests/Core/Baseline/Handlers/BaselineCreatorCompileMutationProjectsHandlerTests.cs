using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Core.Util.FileSystem;

namespace Testura.Mutation.Tests.Core.Baseline.Handlers
{
    [TestFixture]
    public class BaselineCreatorCompileMutationProjectsHandlerTests
    {
        private BaselineCreatorCompileMutationProjectsHandler _baselineCreatorCompileMutationProjectsHandler;
        private Mock<IProjectCompiler> _projectCompiler;
        private Mock<IDirectoryHandler> _directoryHandler;

        [OneTimeSetUp]
        public void SetUp()
        {
            _projectCompiler = new Mock<IProjectCompiler>();
            _directoryHandler = new Mock<IDirectoryHandler>();
            _baselineCreatorCompileMutationProjectsHandler = new BaselineCreatorCompileMutationProjectsHandler(_projectCompiler.Object, _directoryHandler.Object);
        }

        [Test]
        public async Task CompileMutationProjects_WhenCompilerReturnsError_ShouldThrowException()
        {
            var path = "my/path";
            var config = SetUpMockAndWorkspace(path, new CompilationResult
            {
                IsSuccess = false,
                Errors = new List<CompilationError> { new CompilationError { Message = "Compile message", Location = "Test.cs" } }
            });

            var exception = Assert.ThrowsAsync<BaselineException>(async () => await _baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(config, path));
            var compilationException = exception.InnerException as CompilationException;

            Assert.AreEqual(1, compilationException.ErrorMessages.Count);
            Assert.AreEqual("Compile message", compilationException.ErrorMessages.First());
        }

        [Test]
        public async Task CompileMutationProjects_WhenCompilerNotReturnsError_ShouldNotThrowException()
        {
            var path = "my/path";
            var config = SetUpMockAndWorkspace(path, new CompilationResult { IsSuccess = true });

            await _baselineCreatorCompileMutationProjectsHandler.CompileMutationProjectsAsync(config, path);
        }

        private MutationConfig SetUpMockAndWorkspace(string path, CompilationResult compilationResult)
        {
            var workspace = new AdhocWorkspace()
                .CurrentSolution
                .AddProject("TestProject", "TestProject", LanguageNames.CSharp);

            var config = new MutationConfig
            {
                Solution = workspace.Solution,
                MutationProjects = new List<MutationProject>
                {
                    new MutationProject
                    {
                        Project = new SolutionProjectInfo("TestProject", "TestProject.cs", "my/path")
                    }
                }
            };

            _projectCompiler
                .Setup(p => p.CompileAsync(path, workspace.Solution.Projects.First()))
                .Returns(Task.FromResult(compilationResult));

            return config;
        }
    }
}
