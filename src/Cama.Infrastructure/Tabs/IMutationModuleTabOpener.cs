using System.Collections.Generic;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;

namespace Cama.Infrastructure.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(CamaConfig config);

        void OpenDocumentDetailsTab(MutatedDocument document, CamaConfig config);

        void OpenTestRunTab(IList<MutatedDocument> documents, CamaConfig config);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(MFile file, CamaConfig config);

        void OpenFaildToCompileTab(IList<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(List<MutationDocumentResult> completedMutations);
    }
}
