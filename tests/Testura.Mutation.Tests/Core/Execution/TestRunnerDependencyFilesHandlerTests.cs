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

            _config = ConfigCreator.CreateConfig();
            _mockFileSystem = new MockFileSystem();

            CreateFiles
            (_mockFileSystem,
                _config.Solution.Projects.FirstOrDefault(p => p.Name == "MutationProject"),
                "MutationDependency.dll",
                "MutationDependency2.dll",
                "MutationDependency3.dll",
                "MutationSubDirectory");

            CreateFiles
            (_mockFileSystem,
                _config.Solution.Projects.FirstOrDefault(p => p.Name == "TestProject"),
                "TestDependency.dll",
                "TestDependency2.dll",
                "TestDependency3.dll",
                "TestSubDirectory");
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

        private void CreateFiles(
            MockFileSystem mockFileSystem,
            Project project,
            string filename,
            string filename2,
            string filename3,
            string subDirectoryName)
        {
            var directory = Path.GetDirectoryName(project.OutputFilePath);

            mockFileSystem.Directory.CreateDirectory(directory);
            mockFileSystem.File.AppendAllText(project.OutputFilePath, "test");
            mockFileSystem.File.AppendAllText(Path.Combine(directory, filename), "test");
            mockFileSystem.File.AppendAllText(Path.Combine(directory, filename2), "test");

            mockFileSystem.Directory.CreateDirectory(Path.Combine(directory, subDirectoryName));
            mockFileSystem.File.AppendAllText(Path.Combine(directory, subDirectoryName, filename3), "test");
        }
    }
}
