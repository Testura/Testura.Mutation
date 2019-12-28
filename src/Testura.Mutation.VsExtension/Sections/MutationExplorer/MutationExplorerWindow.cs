using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.VsExtension.Sections.MutationExplorer
{
    [Guid("ee8fe630-e2c0-4867-a4ba-112709e71d52")]
    public class MutationExplorerWindow : ToolWindowPane, IVsWindowFrameNotify3
    {
        public MutationExplorerWindow(MutationExplorerWindowControl mutationToolWindowControl)
            : base(null)
        {
            Caption = "Testura mutation explorer";
            Content = mutationToolWindowControl;
        }

        public void InitializeWindow()
        {
            ((MutationExplorerWindowControl)Content).Initialize();
        }

        public void InitializeWindow(IEnumerable<MutationDocumentFilterItem> filterItems)
        {
            ((MutationExplorerWindowControl)Content).Initialize(filterItems);
        }

        public int OnShow(int fShow)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return Microsoft.VisualStudio.VSConstants.S_OK;
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
            ((MutationExplorerWindowControl)Content).Close();

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
    }
}
