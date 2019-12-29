using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    [Guid("2ad3dea2-239c-471b-b9ea-5c5545d8f0c7")]
    public class MutationConfigWindow : ToolWindowPane
    {
        public MutationConfigWindow(MutationConfigWindowControl mutationConfigWindowControl)
            : base(null)
        {
            Caption = "Testura mutation config";
            Content = mutationConfigWindowControl;
        }

        public void InitializeWindow()
        {
            var viewModel = ((MutationConfigWindowControl)Content).DataContext as MutationConfigWindowViewModel;
            viewModel?.Initialize();
        }
    }
}
