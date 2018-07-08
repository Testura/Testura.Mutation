using Cama.Common.Tabs;
using Cama.Module.Mutation.Sections.MutationOverview;

namespace Cama.Module.Mutation.Tab
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenMutationOverviewTab()
        {
            _mainTabContainer.AddTab(new MutationOverviewView());
        }
    }
}
