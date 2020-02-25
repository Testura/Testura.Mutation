using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Core.Execution
{
    [TestFixture]
    public class TestRunnerDependencyFilesHandlerTests
    {
        private string _baseDirectory;
        private MutationConfig _config;
        private MockFileSystem _mockFileSystem;

        [SetUp]
        public void SetUp()
        {
            _baseDirectory = @"C:\Base";

            _mockFileSystem = new MockFileSystem();
            _config = ConfigCreator.CreateConfig(_mockFileSystem);
        }


        [Test]
        public void CreateTestDirectoryAndCopyDependencies_WhenSystemUnderTestPathIsDirectory_ShouldCopyAllFiles()
        { 
            CreateTestDirectoryAndCopyDependenciesAndVerify(6, _config.MutationProjects.First().Project.OutputDirectoryPath);
        }

        [Test]
        public void CreateTestDirectoryAndCopyDependencies_WhenSystemUnderTestPathIsFile_ShouldCopyOnlyDll()
        {
            CreateTestDirectoryAndCopyDependenciesAndVerify(4, Path.Combine(_config.MutationProjects.First().Project.OutputDirectoryPath, _config.MutationProjects.First().Project.OutputFileName));
        }

        private void CreateTestDirectoryAndCopyDependenciesAndVerify(int expectedNumberOfFiles, string systemUnderTestPath)
        {
            var testRunnerDependencyFilesHandler = new TestRunnerDependencyFilesHandler(_mockFileSystem);

            var dllPath = testRunnerDependencyFilesHandler.CreateTestDirectoryAndCopyDependencies(_baseDirectory, _config.TestProjects.First(), systemUnderTestPath);

            Assert.IsFalse(string.IsNullOrEmpty(dllPath));

            var directory = Path.GetDirectoryName(dllPath);

            var files = _mockFileSystem.Directory.GetFiles(directory);

            Assert.AreEqual(expectedNumberOfFiles, files.Length);
            Assert.IsTrue(files.Any(f => f.Contains("MutationProject.dll")));
            Assert.IsTrue(files.Any(f => f.Contains("TestDependency.dll")));
            Assert.IsTrue(files.Any(f => f.Contains("TestDependency2.dll")));

            var directories = _mockFileSystem.Directory.GetDirectories(directory);

            Assert.AreEqual(1, directories.Count());

            var subTestDirectories = _mockFileSystem.Directory.GetFiles(directories.First());
            Assert.AreEqual(1, subTestDirectories.Count());
            Assert.IsTrue(subTestDirectories.First().Contains("TestDependency3.dll"));
        }
    }
}
