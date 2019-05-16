using System;
using System.Collections.Generic;
using Anotar.Log4Net;

namespace Unima.Core.Loggers
{
    public class AzureMutationRunLogger : IMutationRunLogger
    {
        private double _totalNumberOfMutations;

        public void LogBeforeRun(IList<MutationDocument> mutationDocuments)
        {
            _totalNumberOfMutations = mutationDocuments.Count;
            LogProgress((int)_totalNumberOfMutations);
        }

        public void LogBeforeMutation(MutationDocument mutationDocument)
        {
        }

        public void LogAfterMutation(MutationDocument mutationDocument, List<MutationDocumentResult> results, int mutationsRemainingCount)
        {
            LogProgress(mutationsRemainingCount);
        }

        private void LogProgress(int mutationsRemainingCount)
        {
            var progress = Math.Round(mutationsRemainingCount / _totalNumberOfMutations * 100);
            LogTo.Info($"##vso[task.setprogress value={progress};]Mutation execution progress");
        }
    }
}
