using System.Collections.Generic;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("ee8fe630-e2c0-4867-a4ba-112709e71d52")]
    public class MutationExplorerWindow : ToolWindowPane
    {
        public MutationExplorerWindow(MutationExplorerWindowControl mutationToolWindowControl)
            : base(null)
        {
            Caption = "MutationToolWindow";
            Content = mutationToolWindowControl;
        }

        public void InitializeWindow(DTE dte, JoinableTaskFactory packageJoinableTaskFactory)
        {
            ((MutationExplorerWindowControl)Content).Initialize(dte, packageJoinableTaskFactory);
        }

        public void InitializeWindow(DTE dte, JoinableTaskFactory packageJoinableTaskFactory, IEnumerable<string> files)
        {
            ((MutationExplorerWindowControl)Content).Initialize(dte, packageJoinableTaskFactory, files);
        }
    }
}
