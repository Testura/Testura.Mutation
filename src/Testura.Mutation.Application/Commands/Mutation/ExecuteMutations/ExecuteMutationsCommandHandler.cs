using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Loggers;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandHandler : IRequestHandler<ExecuteMutationsCommand, MutationRunResult>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExecuteMutationsCommandHandler));

        private readonly MutationDocumentExecutor _mutationDocumentExecutor;
        private readonly IMutationRunLoggerManager _mutationRunLoggerManager;

        public ExecuteMutationsCommandHandler(MutationDocumentExecutor mutationDocumentExecutor, IMutationRunLoggerManager mutationRunLoggerManager)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
            _mutationRunLoggerManager = mutationRunLoggerManager;
        }

        public async Task<MutationRunResult> Handle(ExecuteMutationsCommand command, CancellationToken cancellationToken)
        {
            var semaphoreSlim = new SemaphoreSlim(command.Config.NumberOfTestRunInstances, command.Config.NumberOfTestRunInstances);

            var results = new List<MutationDocumentResult>();
            var mutationDocuments = new Queue<MutationDocument>(command.MutationDocuments);
            var currentRunningDocuments = new List<Task>();

            var start = DateTime.Now;
            var numberOfMutationsLeft = command.MutationDocuments.Count;

            _mutationRunLoggerManager.Initialize(command.Config.MutationRunLoggers);

            Log.Info($"Total number of mutations generated: {numberOfMutationsLeft}");

            _mutationRunLoggerManager.LogBeforeRun(command.MutationDocuments);

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
                            _mutationRunLoggerManager.LogBeforeMutation(document);
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

                                Interlocked.Decrement(ref numberOfMutationsLeft);
                                _mutationRunLoggerManager.LogAfterMutation(document, results, numberOfMutationsLeft);
                            }

                            semaphoreSlim.Release();
                            command.MutationDocumentCompledtedCallback?.Invoke(result);
                        }
                    }));
                }
            });

            // Wait for the final ones
            await Task.WhenAll(currentRunningDocuments);

            var mutationRunResult = new MutationRunResult(results, cancellationToken.IsCancellationRequested, DateTime.Now - start);

            if (cancellationToken.IsCancellationRequested)
            {
                Log.Info("Mutation run was cancelled");
            }
            else
            {
                Log.Info($"Your mutation score: {mutationRunResult.GetMutationScore()}%");
            }

            return mutationRunResult;
        }
    }
}
