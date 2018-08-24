using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models.Mutation;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.Mutators;
using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;
using Cama.Core.Report;
using Cama.Core.Services;
using Cama.Core.Services.Project;
using Cama.Core.TestRunner;

namespace Cama.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Do();

            System.Console.WriteLine("Waiting...");
            System.Console.ReadLine();
        }

        private static async void Do()
        {
            var projectLoader = new ProjectService();
            var config = await projectLoader.OpenProjectAsync(@"C:\Users\Milleb\Documents\Cama\Projects\NewProject.json");

            var someService = new MutatorCreator(new UnitTestAnalyzer());
            var files = await someService.CreateMutatorsAsync(config, new List<IMutator> { new MathMutator(), new ConditionalBoundaryMutator(), new NegateConditionalMutator(), new ReturnValueMutator() });

            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(), new TestRunner());
            foreach (var mFile in files)
            {
                foreach (var mFileMutatedDocument in mFile.MutatedDocuments)
                {
                    var result = await testRunner.RunTestAsync(config, mFileMutatedDocument);
                }
            }
            

            /*
            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(), new TestRunner());
            var result = await testRunner.RunTestAsync(config, files.Where(f => f.MutatedDocuments.Any()).ToList()[1].MutatedDocuments.FirstOrDefault(), config.TestProjectOutputPath);

            HtmlReport.SaveReport(new List<MutationDocumentResult> { result }, "test.html");
            */
        }
    }
}
