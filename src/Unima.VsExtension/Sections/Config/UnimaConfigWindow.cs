using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Unima.VsExtension.Sections.Config
{
    [Guid("2ad3dea2-239c-471b-b9ea-5c5545d8f0c7")]
    public class UnimaConfigWindow : ToolWindowPane
    {
        public UnimaConfigWindow(UnimaConfigWindowControl unimaConfigWindowControl)
            : base(null)
        {
            Caption = "Unima mutation config";
            Content = unimaConfigWindowControl;
        }
    }
}
