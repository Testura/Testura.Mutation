using System.IO.Abstractions.TestingHelpers;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Tests.Utils.Creators;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject
{
    [TestFixture]
    public class OpenProjectCommandHandlerTests
    {
        private OpenProjectCommandHandler _openProjectCommandHandler;
        private MockFileSystem _fileSystem;
        private MutationConfig _config;
        private MutationFileConfig _fileConfig;

        [OneTimeSetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig(_fileSystem);
            _fileConfig = ConfigCreator.CreateFileConfig();
            
            _openProjectCommandHandler = new OpenProjectCommandHandler(
                    new OpenProjectSolutionHandler(_fileSystem, new SolutionOpenerStub(_config.Solution)),
                    new OpenProjectMutatorsHandler(), 
                    new OpenProjectGitFilterHandler(new MutationDocumentFilterItemGitDiffCreator(new GitDiffStub())),
                    new OpenProjectWorkspaceHandler(), 
                    new OpenProjectBaselineHandler(BaselineCreatorCreator.CreatePositiveBaseline(_fileSystem)));
        }

        [Test]
        public async Task Handle_WhenOpenProject_ShouldGetProject()
        {
            _fileConfig.Git = new GitInfo { GenerateFilterFromDiffWithMaster = true };
            _fileConfig.Filter.FilterItems.Add(new MutationDocumentFilterItem());

            var command = new OpenProjectCommand(_fileConfig);
            var project = await _openProjectCommandHandler.Handle(command, CancellationToken.None);

            Assert.AreEqual(_config.Solution, project.Solution, "Solution does not match");
            Assert.AreEqual(_config.MutationProjects.Count, project.MutationProjects.Count, "Wrong number of mutation projects");
            Assert.AreEqual(_config.TestProjects.Count, project.TestProjects.Count, "Wrong number of test projects");
            Assert.AreEqual("Debug", project.BuildConfiguration, "Wrong build configuration");
            Assert.AreEqual(1, project.BaselineInfos.Count, "Should have one baseline info");
            Assert.AreEqual(3, project.Filter.FilterItems.Count, "Wrong number of filter items");
        }
    }
}
