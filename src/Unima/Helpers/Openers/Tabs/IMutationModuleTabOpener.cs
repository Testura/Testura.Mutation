using System.Collections.Generic;
using Unima.Application;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Execution.Report.Unima;

namespace Unima.Helpers.Openers.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(UnimaConfig config);

        void OpenDocumentDetailsTab(MutationDocument document, UnimaConfig config);

        void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, UnimaConfig config);

        void OpenTestRunTab(UnimaReport report);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(FileMutationsModel file, UnimaConfig config);

        void OpenFaildToCompileTab(IEnumerable<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(IEnumerable<MutationDocumentResult> completedMutations);
    }
}
