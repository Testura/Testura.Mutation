﻿using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Core.Baseline
{
    [TestFixture]
    public class BaselineCreatorTests
    {
        private IFileSystem _fileSystem;
        private MutationConfig _config;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig();
        }

        [Test]
        public async Task CreateBaselineAsync_WhenCreatingBaseline_ShouldGetBaselineInfoAndDeleteBaselineDirectory()
        {
            var baselineCreator = new BaselineCreator(
                _fileSystem,
                new BaselineCreatorCompileMutationProjectsHandler(ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem), _fileSystem),
                new BaselineCreatorRunUnitTestsHandler(TestRunnerClientCreator.CreatePositive(), new TestRunnerDependencyFilesHandler(_fileSystem)),
                new BaselineCreatorLogSummaryHandler());

            var result = await baselineCreator.CreateBaselineAsync(_config);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestProject", result.First().TestProjectName);
            Assert.AreEqual(TimeSpan.FromSeconds(1), result.First().ExecutionTime);
        }

        [Test]
        public async Task CreateBaselineAsync_WhenCreatingBaseline_ShouldDeleteBaseDirectory()
        { 
            var baselineCreator = BaselineCreatorCreator.CreatePositiveBaseline(_fileSystem);
            await baselineCreator.CreateBaselineAsync(_config);
           Assert.IsFalse(_fileSystem.Directory.Exists(baselineCreator.BaselineDirectoryPath));
        }

        [Test]
        public async Task CreateBaselineAsync_WhenCreatingBaselineAndCompileFail_ShouldStillDeleteDirectory()
        {
            var baselineCreator = BaselineCreatorCreator.CreateBaselineWithCompileFail(_fileSystem);
            Assert.ThrowsAsync<BaselineException>(async () => await baselineCreator.CreateBaselineAsync(_config));
            Assert.IsFalse(_fileSystem.Directory.Exists(baselineCreator.BaselineDirectoryPath));
        }

        [Test]
        public async Task CreateBaselineAsync_WhenCreatingBaselineAndTestRunFail_ShouldStillDeleteDirectory()
        {
            var baselineCreator = BaselineCreatorCreator.CreateBaselineWithTestRunFail(_fileSystem);

            Assert.ThrowsAsync<BaselineException>(async () => await baselineCreator.CreateBaselineAsync(_config));
            Assert.IsFalse(_fileSystem.Directory.Exists(baselineCreator.BaselineDirectoryPath));
        }
    }
}
