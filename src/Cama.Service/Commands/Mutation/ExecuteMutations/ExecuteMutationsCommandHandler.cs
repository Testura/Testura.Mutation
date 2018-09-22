using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core;
using Cama.Core.Execution;

namespace Cama.Service.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandHandler : ValidateResponseRequestHandler<ExecuteMutationsCommand, IList<MutationDocumentResult>>
    {
        private readonly MutationDocumentExecutor _mutationDocumentExecutor;

        public ExecuteMutationsCommandHandler(MutationDocumentExecutor mutationDocumentExecutor)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
        }

        public override async Task<IList<MutationDocumentResult>> OnHandle(ExecuteMutationsCommand command, CancellationToken cancellationToken)
        {
            var semaphoreSlim = new SemaphoreSlim(command.Config.TestRunInstancesCount, command.Config.TestRunInstancesCount);
            var results = new List<MutationDocumentResult>();
            var numberOfMutationsLeft = command.MutationDocuments.Count;

            var tasks = command.MutationDocuments.Select((document) => Task.Run(async () =>
            {
                MutationDocumentResult result = null;
                
                try
                {
                    semaphoreSlim.Wait();
                    command.MutationDocumentStartedCallback?.Invoke(document);
                    result = await _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, document);
                }
                catch (Exception ex)
                {
                    LogTo.WarnException($"Unexpected exception when running {document.MutationName}", ex);

                    result = new MutationDocumentResult
                    {
                        Id = document.Id,
                        UnexpectedError = ex.Message
                    };
                }
                finally
                {
                    lock (results)
                    {
                        results.Add(result);
                    }

                    Interlocked.Decrement(ref numberOfMutationsLeft);
                    LogTo.Info($"Number of mutations left: {numberOfMutationsLeft}");
                    semaphoreSlim.Release();
                    command.MutationDocumentCompledtedCallback?.Invoke(result);
                }
            })).ToArray();

            await Task.WhenAll(tasks);

            return results;
        }
    }
}
