using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Loggers;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandHandler : IRequestHandler<ExecuteMutationsCommand, IList<MutationDocumentResult>>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExecuteMutationsCommandHandler));

        private readonly MutationDocumentExecutor _mutationDocumentExecutor;
        private readonly MutationRunLoggerFactory _mutationRunLoggerFactory;

        public ExecuteMutationsCommandHandler(MutationDocumentExecutor mutationDocumentExecutor, MutationRunLoggerFactory mutationRunLoggerFactory)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
            _mutationRunLoggerFactory = mutationRunLoggerFactory;
        }

        public async Task<IList<MutationDocumentResult>> Handle(ExecuteMutationsCommand command, CancellationToken cancellationToken)
        {
            var semaphoreSlim = new SemaphoreSlim(command.Config.NumberOfTestRunInstances, command.Config.NumberOfTestRunInstances);
            var results = new List<MutationDocumentResult>();
            var mutationDocuments = new Queue<MutationDocument>(command.MutationDocuments);
            var currentRunningDocuments = new List<Task>();
            var numberOfMutationsLeft = command.MutationDocuments.Count;
            var mutationRunLoggers = command.Config.MutationRunLoggers?.Select(m => _mutationRunLoggerFactory.GetMutationRunLogger(m)).ToList() ?? new List<IMutationRunLogger>();
            var expectedExecutionTime = GetExpectedExecutionTime(command);

            Log.Info($"Total number of mutations generated: {numberOfMutationsLeft}");
            Log.Info($"Expected execution time: {expectedExecutionTime}");

            mutationRunLoggers.ForEach(m => m.LogBeforeRun(command.MutationDocuments));

            await Task.Run(() =>
            {
                while (mutationDocuments.Any())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Log.Info("Cancellation requested (mutation run)");
                        return;
                    }

                    semaphoreSlim.Wait();
                    var document = mutationDocuments.Dequeue();

                    currentRunningDocuments.Add(Task.Run(async () =>
                    {
                        MutationDocumentResult result = null;

                        try
                        {
                            mutationRunLoggers.ForEach(m => m.LogBeforeMutation(document));
                            command.MutationDocumentStartedCallback?.Invoke(document);

                            var resultTask = _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, document, cancellationToken);
                            result = await resultTask;
                        }
                        catch (Exception ex)
                        {
                            Log.Warn($"Unexpected exception when running {document.MutationName}", ex);
                            Log.Info("Will put it in the unknown category.");

                            result = new MutationDocumentResult
                            {
                                Id = document.Id,
                                UnexpectedError = ex.Message,
                            };
                        }
                        finally
                        {
                            lock (results)
                            {
                                results.Add(result);

                                var survived = results.Count(r => r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess) && r.UnexpectedError == null);
                                var killed = results.Count(r => !r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess) && r.UnexpectedError == null);
                                var compileErrors = results.Count(r => r.CompilationResult != null && !r.CompilationResult.IsSuccess);
                                var unknownErrors = results.Count(r => r.UnexpectedError != null);

                                Interlocked.Decrement(ref numberOfMutationsLeft);
                                Log.Info($"Current progress: {{ Survived: {survived}, Killed: {killed}, CompileErrors: {compileErrors}, UnknownErrors: {unknownErrors}, MutationsLeft: {numberOfMutationsLeft} }}");
                                mutationRunLoggers.ForEach(m => m.LogAfterMutation(document, results, numberOfMutationsLeft));
                            }

                            semaphoreSlim.Release();
                            command.MutationDocumentCompledtedCallback?.Invoke(result);
                        }
                    }));
                }
            });

            // Wait for the final ones
            await Task.WhenAll(currentRunningDocuments);

            if (results.Any() && !cancellationToken.IsCancellationRequested)
            {
                Log.Info($"Your mutation score: {GetMutationScore(results)}%");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                Log.Info("Mutation run was cancelled");
            }

            return results;
        }

        private TimeSpan GetExpectedExecutionTime(ExecuteMutationsCommand command)
        {
            return TimeSpan.FromMinutes(command.Config.BaselineInfos.Sum(b => b.ExecutionTime.TotalMinutes) * command.MutationDocuments.Count / command.Config.NumberOfTestRunInstances);
        }

        private double GetMutationScore(List<MutationDocumentResult> results)
        {
            var validMutations = results.Where(r => r.CompilationResult != null && r.CompilationResult.IsSuccess && r.UnexpectedError == null).ToList();

            if (!validMutations.Any())
            {
                return 100;
            }

            return Math.Round((double)validMutations.Count(r => !r.Survived) / validMutations.Count * 100);
        }
    }
}
