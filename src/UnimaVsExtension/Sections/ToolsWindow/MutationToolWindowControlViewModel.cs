using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Task = System.Threading.Tasks.Task;

namespace UnimaVsExtension.Sections.ToolsWindow
{
    public class MutationToolWindowControlViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private string _solutionPath;
        private IVsOutputWindow _outWindow;
        private IVsOutputWindowPane _customPane;

        public MutationToolWindowControlViewModel(IMediator mediator)
        {
            _mediator = mediator;
            OpenReportCommand = new DelegateCommand(() => Do());
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
