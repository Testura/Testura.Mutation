using System.Windows.Controls;
using Testura.Mutation.Application.Models;

namespace Testura.Mutation.Sections.NewProject
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
