using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace UnimaVsExtension.Sections.ToolsWindow
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
    public class MutationToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutationToolWindow"/> class.
        /// </summary>
        public MutationToolWindow(MutationToolWindowControl toolWindowControl)
            : base(null)
        {
            this.Caption = "MutationToolWindow";
            this.Content = toolWindowControl;
        }

        public void InitializeWindow(string solutionFullName)
        { 
            ((MutationToolWindowControl)Content).Initialize(solutionFullName);
        }
    }
}
