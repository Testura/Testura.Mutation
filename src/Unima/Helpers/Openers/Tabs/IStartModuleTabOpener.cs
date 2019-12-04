using Unima.Application.Models;

namespace Unima.Helpers.Openers.Tabs
{
    public interface IStartModuleTabOpener
    {
        void OpenNewProjectTab();

        void OpenNewProjectFromGitTab();

        void OpenNewProjectTab(GitInfo gitInfo, string solutionPath);
    }
}
