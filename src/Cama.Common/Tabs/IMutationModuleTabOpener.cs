using Cama.Core.Mutation;

namespace Cama.Common.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab();

        void OpenDocumentDetailsTab(MutatedDocument document);
    }
}
