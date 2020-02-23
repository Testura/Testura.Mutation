using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Moq;
using NUnit.Framework;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Tests.Core.Baseline.Handlers
{
    [TestFixture]
    public class BaselineCreatorRunUnitTestsHandlerTests
    {
        private const string BaseLineDirectoryPath = "path";

        private Mock<ITestRunnerClient> _testRunnerClient;
        private BaselineCreatorRunUnitTestsHandler _baselineCreatorRunUnitTestsHandler;
        private MockFileSystem _fileSystem;

        [OneTimeSetUp]
        public void SetUp()
        {
            _testRunnerClient = new Mock<ITestRunnerClient>();
            _fileSystem = new MockFileSystem();

            _baselineCreatorRunUnitTestsHandler = new BaselineCreatorRunUnitTestsHandler(
                _testRunnerClient.Object,
                new TestRunnerDependencyFilesHandler(_fileSystem));
        }

        [Test]
        public async Task RunUnitTests_WhenAndTestsPass_ShouldGetBaselineResults()
        {
            var config = SetUpMockAndWorkspace(new TestSuiteResult
            {
                IsSuccess = true,
                ExecutionTime = TimeSpan.FromSeconds(1),
                TestResults = new List<TestResult>
                {
                    new TestResult { IsSuccess = true }
                }
            });

            var result = await _baselineCreatorRunUnitTestsHandler.RunUnitTests(config, BaseLineDirectoryPath);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestProject", result.First().TestProjectName);
            Assert.AreEqual(TimeSpan.FromSeconds(1), result.First().ExecutionTime);
        }

        [Test]
        public void RunUnitTests_WhenAndTestsFail_ShouldThrowExceptions()
        {
            var config = SetUpMockAndWorkspace(new TestSuiteResult
            {
                IsSuccess = false,
                ExecutionTime = TimeSpan.FromSeconds(1),
                TestResults = new List<TestResult>
                {
                    new TestResult { IsSuccess = false }
                }
            });

            Assert.ThrowsAsync<BaselineException>(async () => await _baselineCreatorRunUnitTestsHandler.RunUnitTests(config, BaseLineDirectoryPath));
        }

        [Test]
        public void RunUnitTests_WhenAndWeCancel_ShouldThrowCancelExceptions()
        {
            var config = SetUpMockAndWorkspace(new TestSuiteResult
            {
                IsSuccess = false,
                ExecutionTime = TimeSpan.FromSeconds(1),
                TestResults = new List<TestResult>
                {
                    new TestResult { IsSuccess = false }
                }
            });

            var cts = new CancellationTokenSource();
            var token = cts.Token;

            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await _baselineCreatorRunUnitTestsHandler.RunUnitTests(config, BaseLineDirectoryPath, token));
        }

        private MutationConfig SetUpMockAndWorkspace(TestSuiteResult tetSuiteResult)
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

                },
                TestProjects = new List<TestProject>
                {
                    new TestProject
                    {
                        Project = new SolutionProjectInfo("TestProject", "TestProject.cs", "my/path"),
                        TestRunner = "dotnet"
                    }
                }
            };


            _testRunnerClient
                .Setup(t => t.RunTestsAsync(
                    config.TestProjects.First().TestRunner,
                    It.IsAny<string>(), 
                    config.DotNetPath,
                    TimeSpan.FromMinutes(config.MaxTestTimeMin),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(tetSuiteResult));

            return config;
        }
    }
}
