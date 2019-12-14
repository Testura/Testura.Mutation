using System.Windows.Controls;
using EnvDTE;

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

        public void Initialize(DTE dte)
        {
           ((MutationExplorerWindowViewModel)DataContext).Initialize(dte);
        }
    }
}