using System.Collections.Generic;
using Cama.Core;
using Cama.Core.Execution.Report.Cama;
using Cama.Service;

namespace Cama.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(CamaConfig config);

        void OpenDocumentDetailsTab(MutationDocument document, CamaConfig config);

        void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, CamaConfig config);

        void OpenTestRunTab(CamaReport report);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(FileMutationsModel file, CamaConfig config);

        void OpenFaildToCompileTab(IEnumerable<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(IEnumerable<MutationDocumentResult> completedMutations);
    }
}
