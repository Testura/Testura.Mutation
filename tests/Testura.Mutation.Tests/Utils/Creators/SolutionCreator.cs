using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Testura.Mutation.Tests.Utils.Creators
{
    internal static class SolutionCreator
    {
        private const string MyMutationDocument = @"namespace MyMutationClassNamespace
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
                ";

        private const string MySecondMutationDocument = @"namespace MyMutationClassNamespace
                {
                   public class MySecondMutationClass 
                   {

                                 public void Do() 
                                 {
                                     var i = 3 * 2;
                                 }
                                }
                }";

        public static Solution CreateDefaultSolution()
        {
            return new AdhocWorkspace()
                .CurrentSolution
                .AddProject(CreateTestProject())
                .AddProject(CreateMutationProject());
        }

        private static ProjectInfo CreateTestProject()
        {
            return ProjectInfo.Create(
                    ProjectId.CreateNewId(),
                    VersionStamp.Create(),
                    "TestProject",
                    "TestProject",
                    LanguageNames.CSharp,
                    outputFilePath: Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Test", "TestProject.dll"));
        }

        private static ProjectInfo CreateMutationProject()
        {
            var projectId = ProjectId.CreateNewId();

            return ProjectInfo.Create(
                projectId,
                VersionStamp.Create(),
                "MutationProject",
                "MutationProject",
                LanguageNames.CSharp,
                outputFilePath: Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Mutation", "MutationProject.dll"),
                documents: new List<DocumentInfo>
                {
                    DocumentInfo.Create(
                        DocumentId.CreateNewId(projectId), 
                        "MyMutationDocument.cs",
                        loader: TextLoader.From(TextAndVersion.Create(SourceText.From(MyMutationDocument), VersionStamp.Default ))),

                    DocumentInfo.Create(
                        DocumentId.CreateNewId(projectId),
                        "MySecondMutationDocument.cs",
                        loader: TextLoader.From(TextAndVersion.Create(SourceText.From(MySecondMutationDocument), VersionStamp.Default ))),
                });
        }
    }
}
