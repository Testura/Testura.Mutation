using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
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
            System.Console.WriteLine("Starting mutation testing");

            if (args.Length < 2)
            {
                System.Console.WriteLine("Missing config path + save path");
                System.Console.ReadLine();
                return;
            }

            

            Do(args[0], args[1]);

            System.Console.ReadLine();
        }

        private static async void Do(string configPath, string savePath)
        {
            var projectLoader = new ProjectService();

            /* var config = await projectLoader.OpenProjectAsync(@"C:\Users\Mille\OneDrive\Dokument\cama\TesturaCode.json"); */
            /* var config = await projectLoader.OpenProjectAsync(@"C:\Users\Milleb\Documents\Cama\Projects\NewProject.json"); */
            /* var config = await projectLoader.OpenProjectAsync(@"C:\Users\Milleb\Documents\Cama\Projects\adsadsadsadsa.json");*/
            var config = await projectLoader.OpenProjectAsync(configPath);

            var someService = new MutatorCreator(new UnitTestAnalyzer());
            var files = await someService.CreateMutatorsAsync(config,
                new List<IMutator>
                {
                    new MathMutator(),
                    new ConditionalBoundaryMutator(),
                    new NegateConditionalMutator(),
                    new ReturnValueMutator()
                });

            var results = await RunTests(files, config);

            /* RtxReport.SaveReport(results, @"C:\Users\Mille\OneDrive\Dokument\cama\Result.trx"); */
            /* RtxReport.SaveReport(results, @"C:\Users\Milleb\Documents\Cama\Result.trx"); */
            RtxReport.SaveReport(results, savePath);
        }


        private static async Task<IList<MutationDocumentResult>> RunTests(IList<MFile> files, CamaRunConfig config)
        {
            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(),
                new TestRunner());
            var results = new List<MutationDocumentResult>();

            var runs = files.SelectMany(f => f.MutatedDocuments).Select((d) => new Task(async () =>
            {
                var result = await testRunner.RunTestAsync(config, d);
                lock (results)
                {
                    results.Add(result);
                }
            })).ToArray();

            var queue = new Queue<Task>(runs);
            var runList = new List<Task>();

            while (queue.Any() || runList.Any())
            {
                if (runList.Count > 4 || (runList.Count > 0 && !queue.Any()))
                {
                    var finishedTask = await Task.WhenAny(runList);
                    runList.Remove(finishedTask);
                }
                else if (queue.Any())
                {
                    var newOne = queue.Dequeue();
                    newOne.Start();
                    runList.Add(newOne);
                }
            }

            return results;
        }
    }
}