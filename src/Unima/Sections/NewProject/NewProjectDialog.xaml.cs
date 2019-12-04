using System.Windows.Controls;
using Unima.Application.Models;

namespace Unima.Sections.NewProject
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectDialog : TabItem
    {
        public NewProjectDialog()
        {
            InitializeComponent();
        }

        public NewProjectDialog(GitInfo gitInfo, string solutionPath)
        {
            InitializeComponent();

            var viewModel = DataContext as NewProjectViewModel;
            viewModel.InitializeWithGitInfo(gitInfo, solutionPath);
        }
    }
}
