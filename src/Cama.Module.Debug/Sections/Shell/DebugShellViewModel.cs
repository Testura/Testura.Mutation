using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cama.Core.Logs;
using Prism.Mvvm;

namespace Cama.Module.Debug.Sections.Shell
{
    public class DebugShellViewModel : BindableBase
    {
        private readonly LogWatcher _logWatcher;

        public DebugShellViewModel(LogWatcher logWatcher)
        {
            _logWatcher = logWatcher;
            _logWatcher.Updated += LogWatcherOnUpdated;
        }

        private void LogWatcherOnUpdated(object sender, EventArgs eventArgs)
        {
        }
    }
}
