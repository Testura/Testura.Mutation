using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Application.Commands.Project.OpenProject.Handlers
{
    [TestFixture]
    public class OpenProjectWorkspaceHandlerTests
    {
        private MockFileSystem _fileSystem;
        private MutationConfig _config;
        private OpenProjectWorkspaceHandler _openProjectWorkspaceHandler;
        private MutationFileConfig _fileConfig;

        [SetUp]
        public void SetUp()
        { 
            _fileSystem = new MockFileSystem();
            _fileConfig = ConfigCreator.CreateFileConfig();
            _config = ConfigCreator.CreateConfig(_fileSystem);

            _openProjectWorkspaceHandler = new OpenProjectWorkspaceHandler();
        }

        [Test]
        public void CreateMutationProjects_WhenCreateMutationProjects_ShouldGetProjects()
        { 
            var projects = _openProjectWorkspaceHandler.CreateMutationProjects(_fileConfig, _config.Solution);

            Assert.AreEqual(1, projects.Count);

            var project = projects.First();
            var expectedProject = _config.MutationProjects.First();

            Assert.AreEqual(expectedProject.Project.Name, project.Project.Name);
            Assert.AreEqual(expectedProject.Project.OutputDirectoryPath, project.Project.OutputDirectoryPath);
            Assert.AreEqual(expectedProject.Project.OutputFileName, project.Project.OutputFileName);
        }

        [Test]
        public void CreateMutationProjects_WhenCreateMutationProjectsAndDontHaveAnyTestProject_ShouldStillGetProjects()
        {
            _fileConfig.TestProjects = null;

            var projects = _openProjectWorkspaceHandler.CreateMutationProjects(_fileConfig, _config.Solution);
            Assert.AreEqual(2, projects.Count);
        }

        [Ignore("Problem with mutation testing")]
        [TestCase("release", "Release", TestName = "CreateMutationProjects_WhenCreateMutationProjectsAndBuildConfigurationIsRelease_ShouldGetRelease")]
        [TestCase("debug", "Debug", TestName = "CreateMutationProjects_WhenCreateMutationProjectsAndBuildConfigurationIsDebug_ShouldGetDebug")]
        [TestCase("null", "Debug", TestName = "CreateMutationProjects_WhenCreateMutationProjectsAndBuildConfigurationIsNull_ShouldGetDebug")]
        public void WhenCreateMutationProjectsAndBuildConfiguration(string buildConfiguration, string expectedBuildConfiguration)
        {
            _fileConfig.BuildConfiguration = buildConfiguration;

            var projects = _openProjectWorkspaceHandler.CreateMutationProjects(_fileConfig, _config.Solution);
            StringAssert.Contains(expectedBuildConfiguration, projects.First().Project.OutputDirectoryPath);
        }

        [Ignore("Problem with mutation testing")]
        [TestCase("release", "Release", TestName = "CreateTestProjects_WhenCreateTestProjectsAndBuildConfigurationIsRelease_ShouldGetRelease")]
        [TestCase("debug", "Debug", TestName = "CreateTestProjects_WhenCreateTestProjectsAndBuildConfigurationIsDebug_ShouldGetDebug")]
        [TestCase("null", "Debug", TestName = "CreateTestProjects_WhenCreateTestProjectsAndBuildConfigurationIsNull_ShouldGetDebug")]
        public void WhenCreateTestProjectsAndBuildConfiguration(string buildConfiguration, string expectedBuildConfiguration)
        {
            _fileConfig.BuildConfiguration = buildConfiguration;

            var projects = _openProjectWorkspaceHandler.CreateTestProjects(_fileConfig, _config.Solution);
            StringAssert.Contains(expectedBuildConfiguration, projects.First().Project.OutputDirectoryPath);
        }

        [Test]
        public void CreateMutationProjects_WhenCreateMutationProjectsAndHaveProjectMapping_ShouldGetProjectMapping()
        {
            _fileConfig.ProjectMappings.Add(new ProjectMapping { ProjectName = "MutationProject", TestProjectNames = new List<string> { "TestProject" }});

            var projects = _openProjectWorkspaceHandler.CreateMutationProjects(_fileConfig, _config.Solution);

            Assert.AreEqual(1, projects.First().MappedTestProjects.Count);
            Assert.AreEqual("TestProject", projects.First().MappedTestProjects.First());
        }


        [Test]
        public void CreateTestProjects_WhenCreateTestProjects_ShouldGetProjects()
        {
            var projects = _openProjectWorkspaceHandler.CreateTestProjects(_fileConfig, _config.Solution);

            Assert.AreEqual(1, projects.Count);

            var project = projects.First();
            var expectedProject = _config.TestProjects.First();

            Assert.AreEqual(expectedProject.Project.Name, project.Project.Name);
            Assert.AreEqual(expectedProject.Project.OutputDirectoryPath, project.Project.OutputDirectoryPath);
            Assert.AreEqual(expectedProject.Project.OutputFileName, project.Project.OutputFileName);
            Assert.AreEqual("DotNet", project.TestRunner);
        }

        [Test]
        public void CreateTestProjects_WhenCreateTestProjectsAndHaveMultipleWithSameName_ShouldNotGetDuplicates()
        {
            _fileConfig.TestProjects.Add(_fileConfig.TestProjects[0]);

            var projects = _openProjectWorkspaceHandler.CreateTestProjects(_fileConfig, _config.Solution);

            Assert.AreEqual(1, projects.Count);
        }

        [Test]
        public void CreateTestProjects_WhenCreateTestProjectsAndCancel_ShouldBeCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => _openProjectWorkspaceHandler.CreateTestProjects(_fileConfig, _config.Solution, token));
        }

        [Test]
        public void CreateMutaitonProjects_WhenCreateMutationProjectsAndCancel_ShouldBeCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            tokenSource.Cancel();

            Assert.Throws<OperationCanceledException>(() => _openProjectWorkspaceHandler.CreateMutationProjects(_fileConfig, _config.Solution, token));
        }
    }
}
