using System.Collections.ObjectModel;
using System.ComponentModel;
using Cama.Core.Logs;
using Prism.Mvvm;

namespace Cama.Module.Debug.Sections.Shell
{
    public class DebugShellViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly LogWatcher _logWatcher;

        public DebugShellViewModel(LogWatcher logWatcher)
        {
            _logWatcher = logWatcher;
            LogText = string.Empty;
            _logWatcher.NewMessage += LogWatcherOnNewMessage;
        }

        public string LogText { get; set; }

        private void LogWatcherOnNewMessage(object sender, string s)
        {
            LogText += s;
        }
    }
}
