using System.Windows.Controls;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationExplorerWindowControl : UserControl
    {
        public MutationExplorerWindowControl()
        {
            this.InitializeComponent();
        }

        public void Initialize(string solutionPath)
        {
           ((MutationExplorerWindowViewModel)DataContext).Initialize(solutionPath);
        }
    }
}