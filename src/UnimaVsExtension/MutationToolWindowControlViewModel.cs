using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using EnvDTE;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Logs;
using Unima.Application.Models;
using Unima.Core.Config;
using Task = System.Threading.Tasks.Task;

namespace UnimaVsExtension
{
    public class MutationToolWindowControlViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private readonly LogWatcher _logWatcher;

        private string _solutionPath;
        private IVsOutputWindow _outWindow;
        private IVsOutputWindowPane _customPane;

        public MutationToolWindowControlViewModel(IMediator mediator, LogWatcher logWatcher)
        {
            _mediator = mediator;
            _logWatcher = logWatcher;
            OpenReportCommand = new DelegateCommand(() => Do());
            _logWatcher.NewMessage  += LogWatcherOnNewMessage;
        }

        private void LogWatcherOnNewMessage(object sender, string e)
        {
            if (_customPane == null)
            {
                _outWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;

                // Use e.g. Tools -> Create GUID to make a stable, but unique GUID for your pane.
                // Also, in a real project, this should probably be a static constant, and not a local variable
                Guid customGuid = new Guid("0F44E2D1-F5FA-4d2d-AB30-22BE8ECD9789");
                string customTitle = "Custom Window Title";
                _outWindow.CreatePane(ref customGuid, customTitle, 1, 1);

                _outWindow.GetPane(ref customGuid, out _customPane);
            }

            _customPane.OutputString(e);
            _customPane.Activate(); // Brings this pane into view
        }

        public DelegateCommand OpenReportCommand { get; set; }

        public async void Do()
        {
            Task.Run(async () =>
            {
                var m = new UnimaFileConfig
                {
                    SolutionPath = _solutionPath,
                    TestProjects = new List<string> {"*.Tests*"},
                    TestRunner = "dotnet"
                };

                var o = Path.GetDirectoryName(_solutionPath);
                var configPath = Path.Combine(o, "mutationConfig.json");

                File.WriteAllText(configPath, JsonConvert.SerializeObject(m));

                var config = await _mediator.Send(new OpenProjectCommand(configPath));
            });
        }

        public void Initialize(string solutionPath)
        {
            _solutionPath = solutionPath;
        }
    }
}
