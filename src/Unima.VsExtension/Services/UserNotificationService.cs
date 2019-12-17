using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.Shell;

namespace Unima.VsExtension.Services
{
    public class UserNotificationService
    {
        private readonly SVsServiceProvider _serviceProvider;
        private readonly IProjectThreadingService _threadingService;

        public UserNotificationService(SVsServiceProvider serviceProvider, IProjectThreadingService threadingService)
        {
            _serviceProvider = serviceProvider;
            _threadingService = threadingService;
        }

        /*
        public void ShowMessage(string message)
        {
            ShowMessageBox(message, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_OK, VSConstants.MessageBoxResult.IDYES);
        }

        public bool Confirm(string message)
        {
            VSConstants.MessageBoxResult result = ShowMessageBox(message, OLEMSGICON.OLEMSGICON_QUERY, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, VSConstants.MessageBoxResult.IDYES);

            return result == VSConstants.MessageBoxResult.IDYES;
        }

        public void ShowWarning(string warning)
        {
            ShowMessageBox(warning, OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK);
        }

        public void ShowError(string error)
        {
            ShowMessageBox(error, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK);
        }

        private VSConstants.MessageBoxResult ShowMessageBox(string message, OLEMSGICON icon, OLEMSGBUTTON button, VSConstants.MessageBoxResult defaultResult = VSConstants.MessageBoxResult.IDOK)
        {
            _threadingService.VerifyOnUIThread();

            if (VsShellUtilities.IsInAutomationFunction(_serviceProvider))
            {
                return defaultResult;
            }

            return (VSConstants.MessageBoxResult)VsShellUtilities.ShowMessageBox(_serviceProvider, message, null, icon, button, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
        */
    }
}
