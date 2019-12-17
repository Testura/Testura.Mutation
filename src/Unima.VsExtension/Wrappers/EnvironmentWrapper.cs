using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Unima.VsExtension.Services;

namespace Unima.VsExtension.Wrappers
{
    public class EnvironmentWrapper
    {
        private readonly AsyncPackage _asyncPackage;

        public EnvironmentWrapper(
            DTE dte,
            JoinableTaskFactory joinableTaskFactory,
            AsyncPackage asyncPackage,
            UserNotificationService userNotificationService)
        {
            _asyncPackage = asyncPackage;
            Dte = dte;
            JoinableTaskFactory = joinableTaskFactory;
            UserNotificationService = userNotificationService;
        }

        public DTE Dte { get; }

        public JoinableTaskFactory JoinableTaskFactory { get; }

        public UserNotificationService UserNotificationService { get; }

        public void OpenWindow<T>()
        {
            JoinableTaskFactory.Run(async () =>
            {
                var window = await _asyncPackage.FindToolWindowAsync(typeof(T), 0, true, _asyncPackage.DisposalToken);

                if (window?.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                await _asyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();

                var windowFrame = (IVsWindowFrame)window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            });
        }
    }
}
