using System.Collections.Generic;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Mutators;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;
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
