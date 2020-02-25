﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Tests.Utils.Creators;
using Testura.Mutation.Tests.Utils.Stubs;

namespace Testura.Mutation.Tests.Core.Execution
{
    [TestFixture]
    public class MutationDocumentExecutorTests
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
        public async Task ExecuteMutationAsync_WhenExecuteMutationAndTestFails_ShouldSetMutationsAsNotSurvived()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreateNegative();

            var mutationDocumentExecutor = new MutationDocumentExecutor(compiler, _dependency, testRunner, _fileSystem);
            var result = await mutationDocumentExecutor.ExecuteMutationAsync(_config, _mutationDocument);

            Assert.IsFalse(result.Survived);
        }

        [Test]
        public async Task ExecuteMutationAsync_WhenExecuteMutationAndTestsPass_ShouldSetMutationsAsSurvived()
        {
            var compiler = ProjectCompilerCreator.CreatePositiveCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreatePositive();

            var mutationDocumentExecutor = new MutationDocumentExecutor(compiler, _dependency, testRunner, _fileSystem);
            var result = await mutationDocumentExecutor.ExecuteMutationAsync(_config, _mutationDocument);

            Assert.IsTrue(result.Survived);
        }

        [Test]
        public async Task ExecuteMutationAsync_WhenExecuteMutationAndCompileFail_ShouldSetMutationsAsSurvived()
        {
            var compiler = ProjectCompilerCreator.CreateNegativeCompiler(_fileSystem);
            var testRunner = TestRunnerClientCreator.CreatePositive();

            var mutationDocumentExecutor = new MutationDocumentExecutor(compiler, _dependency, testRunner, _fileSystem);
            var result = await mutationDocumentExecutor.ExecuteMutationAsync(_config, _mutationDocument);

            Assert.IsFalse(result.Survived);
            Assert.IsFalse(result.CompilationResult.IsSuccess);
        }
    }
}
