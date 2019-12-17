using EnvDTE;
using Microsoft.VisualStudio.Threading;
using Unima.VsExtension.Services;

namespace Unima.VsExtension.Wrappers
{
    public class EnvironmentWrapper
    {
        public EnvironmentWrapper(DTE dte, JoinableTaskFactory joinableTaskFactory, UserNotificationService userNotificationService)
        {
            Dte = dte;
            JoinableTaskFactory = joinableTaskFactory;
            UserNotificationService = userNotificationService;
        }

        public DTE Dte { get; }

        public JoinableTaskFactory JoinableTaskFactory { get; }

        public UserNotificationService UserNotificationService { get; }
    }
}
