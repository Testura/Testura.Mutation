using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Mutators;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class ConfigCreator
    {
        public static MutationConfig CreateConfig(IFileSystem fileSystem = null)
        {
            var solution = SolutionCreator.CreateDefaultSolution();

            if (fileSystem != null)
            {
                CreateFiles
                (fileSystem,
                    solution.Projects.FirstOrDefault(p => p.Name == "MutationProject"),
                    "MutationDependency.dll",
                    "MutationDependency2.dll",
                    "MutationDependency3.dll",
                    "MutationSubDirectory");

                CreateFiles
                (fileSystem,
                    solution.Projects.FirstOrDefault(p => p.Name == "TestProject"),
                    "TestDependency.dll",
                    "TestDependency2.dll",
                    "TestDependency3.dll",
                    "TestSubDirectory");
            }

            return new MutationConfig
            {
                Solution = solution,
                NumberOfTestRunInstances = 1,
                Mutators = new List<IMutator>
                {
                    new ConditionalBoundaryMutator(),
                    new IncrementsMutator(),
                    new MathMutator(),
                    new MethodCallMutator(),
                    new NegateConditionalMutator(),
                    new NegateTypeCompabilityMutator(),
                    new ReturnValueMutator()
                },
                MutationProjects = new List<MutationProject>
                {
                    new MutationProject
                    {
                        Project = new SolutionProjectInfo("MutationProject", "MutationProject.csproj", solution.Projects.FirstOrDefault(p => p.Name == "MutationProject").OutputFilePath)
                    }
                },
                TestProjects = new List<TestProject>
                {
                    new TestProject
                    {
                        Project = new SolutionProjectInfo("TestProject", "TestProject.csproj", solution.Projects.FirstOrDefault(p => p.Name == "TestProject").OutputFilePath),
                        TestRunner = "dotnet"
                    }
                }

            };
        }

        private static void CreateFiles(
            IFileSystem mockFileSystem,
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
