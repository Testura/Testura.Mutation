using System.Collections.Generic;
using Cama.Core.Config;
using Cama.Core.Models;
using Cama.Core.Mutation.Models;
using Cama.Core.Report.Cama;
using Cama.Infrastructure.Models;

namespace Cama.Infrastructure.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(CamaConfig config);

        void OpenDocumentDetailsTab(MutationDocument document, CamaConfig config);

        void OpenTestRunTab(IList<MutationDocument> documents, CamaConfig config);

        void OpenTestRunTab(CamaReport report);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(FileMutationsModel file, CamaConfig config);

        void OpenFaildToCompileTab(IList<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(List<MutationDocumentResult> completedMutations);
    }
}
