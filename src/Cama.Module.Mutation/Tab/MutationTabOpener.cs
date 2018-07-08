using Cama.Common.Tabs;
using Cama.Core.Mutation;
using Cama.Module.Mutation.Sections.Details;
using Cama.Module.Mutation.Sections.Overview;

namespace Cama.Module.Mutation.Tab
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab()
        {
            _mainTabContainer.AddTab(new MutationOverviewView());
        }

        public void OpenDocumentDetailsTab(MutatedDocument document)
        {
            _mainTabContainer.AddTab(new MutationDetailsView(document));
        }
    }
}
