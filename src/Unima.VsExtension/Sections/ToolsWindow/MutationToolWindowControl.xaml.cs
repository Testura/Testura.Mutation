using System.Windows.Controls;

namespace Unima.VsExtension.Sections.ToolsWindow
{
    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationToolWindowControl : UserControl
    {
        public MutationToolWindowControl()
        {
            this.InitializeComponent();
        }

        public void Initialize(string solutionPath)
        {
           ((MutationToolWindowViewModel)DataContext).Initialize(solutionPath);
        }
    }
}