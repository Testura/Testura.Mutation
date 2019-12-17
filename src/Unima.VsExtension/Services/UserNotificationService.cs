using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

namespace Unima.VsExtension.Services
{
    public class UserNotificationService
    {
        private readonly AsyncPackage _asyncPackage;
        private readonly JoinableTaskFactory _joinableTaskFactory;

        public UserNotificationService(AsyncPackage asyncPackage, JoinableTaskFactory joinableTaskFactory)
        {
            _asyncPackage = asyncPackage;
            _joinableTaskFactory = joinableTaskFactory;
        }

        public void ShowMessage(string message)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                ShowMessageBox(message, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OK, VSConstants.MessageBoxResult.IDYES);
            });
        }

        public bool Confirm(string message)
        {
            return _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                VSConstants.MessageBoxResult result = ShowMessageBox(message, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, VSConstants.MessageBoxResult.IDYES);

                return result == VSConstants.MessageBoxResult.IDYES;
            });
        }

        public void ShowWarning(string warning)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                ShowMessageBox(warning, OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK);
            });
        }

        public void ShowError(string error)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                ShowMessageBox(error, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK);
            });
        }

        private VSConstants.MessageBoxResult ShowMessageBox(string message, OLEMSGICON icon, OLEMSGBUTTON button, VSConstants.MessageBoxResult defaultResult = VSConstants.MessageBoxResult.IDOK)
        {
            if (VsShellUtilities.IsInAutomationFunction(_asyncPackage))
            {
                return defaultResult;
            }

            return (VSConstants.MessageBoxResult)VsShellUtilities.ShowMessageBox(_asyncPackage, message, null, icon, button, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
