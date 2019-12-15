using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

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

        public void InitializeWindow(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            ((UnimaConfigWindowControl)Content).Initialize(dte, joinableTaskFactory);
        }
    }
}
