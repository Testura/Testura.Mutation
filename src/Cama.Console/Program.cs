using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.Mutators;
using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;
using Cama.Core.Report;
using Cama.Core.Report.Markdown;
using Cama.Core.Report.Trx;
using Cama.Core.Services;
using Cama.Core.Services.Project;
using Cama.Core.TestRunner;

namespace Cama.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("   ____    _    __  __    _  ");
            System.Console.WriteLine("  / ___|  / \\  |  \\/  |  / \\");
            System.Console.WriteLine(" | |     / _ \\ | |\\/| | / _ \\ ");
            System.Console.WriteLine(" | |___ / ___ \\| |  | |/ ___ \\ ");
            System.Console.WriteLine("  \\____/_/   \\_\\_|  |_/_/   \\_\\");
            System.Console.WriteLine("");

            LogTo.Info("Starting mutation testing");

            if(args.Length < 1)
                throw new ArgumentException("Path to cama config is required.");

            if(args.Length < 2)
                throw new ArgumentException("Output path is required.");

            var success = await ExecuteCama(args[0], args[1]);

            if (!success)
            {
                Environment.Exit(-1);
            }

            Environment.Exit(0);
        }

        private static async Task<bool> ExecuteCama(string configPath, string savePath)
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

            new TrxReportCreator().SaveReport(savePath, results);
            new MarkdownReportCreator().SaveReport(Path.ChangeExtension(savePath, ".md"), results);

            return !results.Any(r => r.Survived);
        }


        private static async Task<IList<MutationDocumentResult>> RunTests(IList<MFile> files, CamaConfig config)
        {
            var semaphoreSlim = new SemaphoreSlim(4, 4);
            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(), new NUnitTestRunner());
            var results = new List<MutationDocumentResult>();
            var numberOfMutationsLeft = files.SelectMany(f => f.MutatedDocuments).Count();

            var tasks = files.SelectMany(f => f.MutatedDocuments).Select((d) => Task.Run(async () =>
            {
                try
                {
                    semaphoreSlim.Wait();
                    var result = await testRunner.RunTestAsync(config, d);
                    lock (results)
                    {
                        results.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    LogTo.WarnException($"Unexpected exception when running {d.MutationName}", ex);
                }
                finally
                {
                    Interlocked.Decrement(ref numberOfMutationsLeft);
                    LogTo.Info($"Number of mutations left: {numberOfMutationsLeft}");
                    semaphoreSlim.Release();
                }
            })).ToArray();

            await Task.WhenAll(tasks);
            
            return results;
        }
    }
}