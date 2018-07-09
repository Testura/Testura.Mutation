using System.Collections.Generic;
using Cama.Core.Models.Mutation;

namespace Cama.Infrastructure.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab();

        void OpenDocumentDetailsTab(MutatedDocument document);

        void OpenTestRunTab(IList<MutatedDocument> documents);
    }
}
