using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using log4net;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Core.Execution
{
    public class TestRunnerDependencyFilesHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TestRunnerDependencyFilesHandler));
        private readonly IFileSystem _fileSystem;

        public TestRunnerDependencyFilesHandler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string CreateTestDirectoryAndCopyDependencies(string baseDirectory, TestProject testProject, string systemUnderTestPath)
        {
            var testDirectoryPath = Path.Combine(baseDirectory, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(testDirectoryPath, testProject.Project.OutputFileName);

            _fileSystem.Directory.CreateDirectory(testDirectoryPath);

            // Copy all files from the original test directory to our created test directory
            CopyDependencies(testProject.Project.OutputDirectoryPath, testDirectoryPath);

            // Copy all dlls that we want to use when running the tests.
            CopySystemUnderTest(systemUnderTestPath, testDirectoryPath);

            return testDllPath;
        }

        private void CopySystemUnderTest(string systemUnderTestPath, string testDirectoryPath)
        {
            var attr = _fileSystem.File.GetAttributes(systemUnderTestPath);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (var file in _fileSystem.Directory.EnumerateFiles(systemUnderTestPath))
                {
                    _fileSystem.File.Copy(file, Path.Combine(testDirectoryPath, Path.GetFileName(file)), true);
                }
            }
            else
            {
                _fileSystem.File.Copy(systemUnderTestPath, Path.Combine(testDirectoryPath, Path.GetFileName(systemUnderTestPath)), true);
            }
        }

        private void CopyDependencies(string path, string targetPath)
        {
            var files = 0;

            try
            {
                _fileSystem.Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .ForAll(from =>
                    {
                        try
                        {
                            var to = from.Replace(path, targetPath);

                            // Create directories if need
                            var toSubFolder = Path.GetDirectoryName(to);
                            if (!string.IsNullOrWhiteSpace(toSubFolder))
                            {
                                _fileSystem.Directory.CreateDirectory(toSubFolder);
                            }

                            files++;
                            _fileSystem.File.Copy(from, to, true);
                        }
                        catch (Exception ex)
                        {
                           Log.Warn("Failed top copy dependency", ex);
                        }
                    });
            }
            catch (Exception ex)
            {
                Log.Warn("Failed top copy dependency", ex);
            }

            if (files == 0)
            {
                Log.Warn($"Did not find any files to copy at \"{path}\". Make sure that you built the solution before running mutation tests.");
            }
        }
    }
}
