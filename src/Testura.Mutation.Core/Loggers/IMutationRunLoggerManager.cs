using System.Collections.Generic;

namespace Testura.Mutation.Core.Loggers
{
    public interface IMutationRunLoggerManager
    {
        void Initialize(IEnumerable<MutationRunLoggerKind> mutationRunLoggerKinds);

        void LogBeforeRun(IList<MutationDocument> mutationDocuments);

        void LogBeforeMutation(MutationDocument mutationDocument);

        void LogAfterMutation(MutationDocument mutationDocument, List<MutationDocumentResult> results, int mutationsRemainingCount);
    }
}