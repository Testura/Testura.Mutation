using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Unima.Application.Logs;

namespace Unima.VsExtension.Services
{
    public class OutputLoggerService
    {
        private const string Guid = "8430b3ca-0c67-427e-8844-cc59c01c754c";
        private const string WindowTitle = "Mutation output window";

        private readonly JoinableTaskFactory _joinableTaskFactory;
        private readonly LogWatcher _logWatcher;
        private IVsOutputWindowPane _customPane;

        public OutputLoggerService(JoinableTaskFactory joinableTaskFactory, LogWatcher logWatcher)
        {
            _joinableTaskFactory = joinableTaskFactory;
            _logWatcher = logWatcher;
        }

        public void StartLogger()
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                var customGuid = new Guid(Guid);

                outWindow.CreatePane(ref customGuid, WindowTitle, 1, 1);
                outWindow.GetPane(ref customGuid, out _customPane);

                _logWatcher.NewMessage += LogWatcherOnNewMessage;
            });
        }

        private void LogWatcherOnNewMessage(object sender, string e)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                _customPane.OutputString(e);
                _customPane.Activate();
            });
        }
    }
}