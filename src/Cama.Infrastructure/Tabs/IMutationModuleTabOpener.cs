using System.Collections.Generic;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;

namespace Cama.Infrastructure.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(CamaConfig config);

        void OpenDocumentDetailsTab(MutatedDocument document);

        void OpenTestRunTab(IList<MutatedDocument> documents);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(MFile file);
    }
}
