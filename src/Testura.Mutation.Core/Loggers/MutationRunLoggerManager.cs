using System;
using System.Collections.Generic;
using System.Linq;
using Testura.Mutation.Core.Loggers.LoggerKinds;

namespace Testura.Mutation.Core.Loggers
{
    public class MutationRunLoggerManager : IMutationRunLoggerManager
    {
        private List<IMutationRunLogger> _mutationRunLoggers;

        public void Initialize(IEnumerable<MutationRunLoggerKind> mutationRunLoggerKinds)
        {
            _mutationRunLoggers = new List<IMutationRunLogger>
            {
                new StatusMutationRunLogger()
            };

            if (mutationRunLoggerKinds == null)
            {
                return;
            }

            _mutationRunLoggers.AddRange(mutationRunLoggerKinds.Select(GetMutationRunLogger));
        }

        public void LogBeforeRun(IList<MutationDocument> mutationDocuments)
        {
            foreach (var mutationRunLogger in _mutationRunLoggers)
            {
                mutationRunLogger.LogBeforeRun(mutationDocuments);
            }
        }

        public void LogBeforeMutation(MutationDocument mutationDocument)
        {
            foreach (var mutationRunLogger in _mutationRunLoggers)
            {
                mutationRunLogger.LogBeforeMutation(mutationDocument);
            }
        }

        public void LogAfterMutation(MutationDocument mutationDocument, List<MutationDocumentResult> results, int mutationsRemainingCount)
        {
            foreach (var mutationRunLogger in _mutationRunLoggers)
            {
                mutationRunLogger.LogAfterMutation(mutationDocument, results, mutationsRemainingCount);
            }
        }

        private IMutationRunLogger GetMutationRunLogger(MutationRunLoggerKind mutationRunLoggerKind)
        {
            switch (mutationRunLoggerKind)
            {
                case MutationRunLoggerKind.Azure:
                    return new AzureMutationRunLogger();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationRunLoggerKind), mutationRunLoggerKind, null);
            }
        }
    }
}
