using Testura.Mutation.Application.Models;
using Testura.Mutation.Sections.NewProject;
using NewProjectDialog = Testura.Mutation.Sections.NewProject.NewProjectDialog;

namespace Testura.Mutation.Wpf.Helpers.Openers.Tabs
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
