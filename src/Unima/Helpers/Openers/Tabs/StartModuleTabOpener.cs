using Unima.Application.Models;
using Unima.Sections.NewProject;
using NewProjectDialog = Unima.Sections.NewProject.NewProjectDialog;

namespace Unima.Helpers.Openers.Tabs
{
    public class StartModuleTabOpener : IStartModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public StartModuleTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenNewProjectTab(GitInfo gitInfo, string solutionPath)
        {
            _mainTabContainer.AddTab(new NewProjectDialog(gitInfo, solutionPath));
        }

        public void OpenNewProjectTab()
        {
            _mainTabContainer.AddTab(new NewProjectDialog());
        }

        public void OpenNewProjectFromGitTab()
        {
            _mainTabContainer.AddTab(new NewProjectFromGitDialog());
        }
    }
}
