using System.Collections.Generic;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Tests.Utils.Creators
{
    public static class ConfigCreator
    {
        public static MutationConfig CreateConfig()
        {
            var solution = SolutionCreator.CreateDefaultSolution();
            return new MutationConfig
            {
                Solution = solution,
                MutationProjects = new List<MutationProject>
                {
                    new MutationProject
                    {
                        Project = new SolutionProjectInfo("MutationProject", "MutationProject.csproj", "my/path")
                    }
                },
                TestProjects = new List<TestProject>
                {
                    new TestProject
                    {
                        Project = new SolutionProjectInfo("TestProject", "TestProject.csproj", "my/path"),
                        TestRunner = "dotnet"
                    }
                }

            };
        }
    }
}
