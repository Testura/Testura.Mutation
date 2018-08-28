using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("Starting mutation testing");

            if(args.Length < 1)
                throw new ArgumentException("Path to cama config is required.");

            if(args.Length < 2)
                throw new ArgumentException("Output path is required.");

            await ExecuteCama(args[0], args[1]);
        }

        private static async Task ExecuteCama(string configPath, string savePath)
        {
            var projectLoader = new ProjectService();
            var mutatorCreator = new MutatorCreator(new UnitTestAnalyzer());

            var config = await projectLoader.OpenProjectAsync(configPath);
            var files = await mutatorCreator.CreateMutatorsAsync(config,
                new List<IMutator>
                {
                    new MathMutator(),
                    new ConditionalBoundaryMutator(),
                    new NegateConditionalMutator(),
                    new ReturnValueMutator()
                });

            var results = await RunTests(files, config);
            TrxReport.SaveReport(results, savePath);
        }


        private static async Task<IList<MutationDocumentResult>> RunTests(IList<MFile> files, CamaConfig config)
        {
            var semaphoreSlim = new SemaphoreSlim(4, 4);
            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(), new TestRunner());
            var results = new List<MutationDocumentResult>();

            await Task.WhenAll(files.SelectMany(f => f.MutatedDocuments).Select((d) => Task.Run(async () =>
            {
                semaphoreSlim.Wait();
                var result = await testRunner.RunTestAsync(config, d);
                lock (results)
                {
                    results.Add(result);
                }

                semaphoreSlim.Release();
            })).ToArray());
            
            return results;
        }
    }
}