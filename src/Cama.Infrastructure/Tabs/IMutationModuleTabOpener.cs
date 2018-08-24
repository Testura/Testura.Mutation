using System.Collections.Generic;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;

namespace Cama.Infrastructure.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(CamaRunConfig config);

        void OpenDocumentDetailsTab(MutatedDocument document, CamaRunConfig config);

        void OpenTestRunTab(IList<MutatedDocument> documents, CamaRunConfig config);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(MFile file, CamaRunConfig config);

        void OpenFaildToCompileTab(IList<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(List<MutationDocumentResult> completedMutations);
    }
}
