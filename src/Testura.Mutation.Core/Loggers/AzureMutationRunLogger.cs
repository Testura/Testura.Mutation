using System;
using System.Collections.Generic;
using log4net;

namespace Testura.Mutation.Core.Loggers
{
    public class AzureMutationRunLogger : IMutationRunLogger
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AzureMutationRunLogger));

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
            var progress = 100 - Math.Round(mutationsRemainingCount / _totalNumberOfMutations * 100);
            Log.Info($"##vso[task.setprogress value={progress};]Mutation execution progress");
        }
    }
}
