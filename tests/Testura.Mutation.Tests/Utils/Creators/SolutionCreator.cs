using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Testura.Mutation.Tests.Utils.Creators
{
    internal static class SolutionCreator
    {
        public static Solution CreateDefaultSolution()
        {
            var mutationProject = new AdhocWorkspace()
                .CurrentSolution.AddProject("TestProject", "TestProject", LanguageNames.CSharp)
                .Solution.AddProject("MutationProject", "MutationProject", LanguageNames.CSharp);

            mutationProject = mutationProject.AddDocument("MyMutationDocument.cs", SourceText.From(@"         
                namepace MyMutationClassNamespace
                {
                   public class MyMutationClass 
                   {

                                 public void Do() 
                                 {
                                     var i = 1 + 2;
                                     if(i == 3)
                                     {
                                     }
                                 }
                                }
                }
                ")).Project;

            mutationProject = mutationProject.AddDocument("MySecondMutationDocument.cs", SourceText.From(@"         
                namepace MyMutationClassNamespace
                {
                   public class MySecondMutationClass 
                   {

                                 public void Do() 
                                 {
                                     var i = 3 * 2;
                                 }
                                }
                }
                ")).Project;

            return mutationProject.Solution;
        }
    }
}
