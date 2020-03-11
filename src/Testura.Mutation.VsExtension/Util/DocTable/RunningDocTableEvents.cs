using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Testura.Mutation.VsExtension.Services;

namespace Testura.Mutation.VsExtension.Util.DocTable
{
    public class RunningDocTableEvents : IVsRunningDocTableEvents3
    {
        private readonly EnvironmentService _environmentService;
        private readonly RunningDocumentTable _runningDocumentTable;

        public RunningDocTableEvents(EnvironmentService environmentService, Package package)
        {
            _environmentService = environmentService;
            _runningDocumentTable = new RunningDocumentTable(package);
            _runningDocumentTable.Advise(this);
        }

        public delegate void OnBeforeSaveHandler(object sender, Document document);

        public event OnBeforeSaveHandler BeforeSave;

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            if (BeforeSave == null)
            {
                return VSConstants.S_OK;
            }

            var document = FindDocumentByCookie(docCookie);
            if (document == null)
            {
                return VSConstants.S_OK;
            }

            BeforeSave(this, FindDocumentByCookie(docCookie));
            return VSConstants.S_OK;
        }

        private Document FindDocumentByCookie(uint docCookie)
        {
            var documentInfo = _runningDocumentTable.GetDocumentInfo(docCookie);

            return _environmentService.JoinableTaskFactory.Run(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();
                #pragma warning disable VSTHRD010
                return _environmentService.Dte.Documents.Cast<Document>().FirstOrDefault(doc => doc.FullName == documentInfo.Moniker);
            });
        }
    }
}
