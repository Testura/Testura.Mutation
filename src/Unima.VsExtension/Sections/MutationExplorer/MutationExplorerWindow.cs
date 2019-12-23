using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Unima.Core.Creator.Filter;
using Unima.VsExtension.MutationHighlight;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    [Guid("ee8fe630-e2c0-4867-a4ba-112709e71d52")]
    public class MutationExplorerWindow : ToolWindowPane, IVsWindowFrameNotify3
    {
        private readonly MutationCodeHighlightHandler _mutationCodeHighlightHandler;

        public MutationExplorerWindow(
            MutationExplorerWindowControl mutationToolWindowControl,
            MutationCodeHighlightHandler mutationCodeHighlightHandler)
            : base(null)
        {
            _mutationCodeHighlightHandler = mutationCodeHighlightHandler;
            Caption = "Unima mutation explorer";
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
            _mutationCodeHighlightHandler.ClearHighlights();

            return Microsoft.VisualStudio.VSConstants.S_OK;
        }
    }
}
