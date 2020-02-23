using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Tests.Utils.Creators
{
    internal static class SolutionCreator
    {
        public static Solution CreateDefaultSolution()
        {
            return new AdhocWorkspace()
                .CurrentSolution.AddProject("TestProject", "TestProject", LanguageNames.CSharp)
                .Solution.AddProject("MutationProject", "MutationProject", LanguageNames.CSharp)
                .Solution;
        }
    }
}
