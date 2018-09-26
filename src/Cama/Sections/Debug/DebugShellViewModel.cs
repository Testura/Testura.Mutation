using System.ComponentModel;
using Cama.Service.Logs;
using Prism.Mvvm;

namespace Cama.Sections.Debug
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
