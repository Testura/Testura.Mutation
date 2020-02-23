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
using Testura.Mutation.Tests.Utils.Creators;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Core.Baseline.Handlers
{
    [TestFixture]
    public class BaselineCreatorRunUnitTestsHandlerTests
    {
        private const string BaseLineDirectoryPath = "path";
        private MockFileSystem _fileSystem;
        private TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private MutationConfig _config;

        [OneTimeSetUp]
        public void SetUp()
        {
            _config = ConfigCreator.CreateConfig();
            _fileSystem = new MockFileSystem();
            _testRunnerDependencyFilesHandler = new TestRunnerDependencyFilesHandler(_fileSystem);
        }

        [Test]
        public async Task RunUnitTests_WhenAndTestsPass_ShouldGetBaselineResults()
        {
            var baselineCreatorRunUnitTestsHandler = new BaselineCreatorRunUnitTestsHandler(
                TestRunnerClientCreator.CreatePositive(),
                _testRunnerDependencyFilesHandler);

            var result = await baselineCreatorRunUnitTestsHandler.RunUnitTests(_config, BaseLineDirectoryPath);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestProject", result.First().TestProjectName);
            Assert.AreEqual(TimeSpan.FromSeconds(1), result.First().ExecutionTime);
        }

        [Test]
        public void RunUnitTests_WhenAndTestsFail_ShouldThrowExceptions()
        {
            var baselineCreatorRunUnitTestsHandler = new BaselineCreatorRunUnitTestsHandler(
                TestRunnerClientCreator.CreateNegative(),
                _testRunnerDependencyFilesHandler);

            Assert.ThrowsAsync<BaselineException>(async () => await baselineCreatorRunUnitTestsHandler.RunUnitTests(_config, BaseLineDirectoryPath));
        }

        [Test]
        public void RunUnitTests_WhenAndWeCancel_ShouldThrowCancelExceptions()
        {
            var baselineCreatorRunUnitTestsHandler = new BaselineCreatorRunUnitTestsHandler(
                TestRunnerClientCreator.CreateNegative(),
                _testRunnerDependencyFilesHandler);

            var cts = new CancellationTokenSource();
            var token = cts.Token;

            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await baselineCreatorRunUnitTestsHandler.RunUnitTests(_config, BaseLineDirectoryPath, token));
        }
    }
}
