using Cama.Tabs;
using NewProjectDialog = Cama.Sections.NewProject.NewProjectDialog;

namespace Cama.Tab
{
    public class StartModuleTabOpener : IStartModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public StartModuleTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenNewProjectTab()
        {
            _mainTabContainer.AddTab(new NewProjectDialog());
        }
    }
}
