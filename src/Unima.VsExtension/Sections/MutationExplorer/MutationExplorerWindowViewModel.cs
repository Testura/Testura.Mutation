using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using EnvDTE;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Commands.Mutation.CreateMutations;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Unima.Core;
using Unima.Wpf.Shared.Models;
using Task = System.Threading.Tasks.Task;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private string _solutionPath;
        private DTE _dte;

        public MutationExplorerWindowViewModel(IMediator mediator)
        {
            _mediator = mediator;
            Mutations = new ObservableCollection<TestRunDocument>();
            RunMutationsCommand = new DelegateCommand(() => Do());
            MutationSelectedCommand = new DelegateCommand<TestRunDocument>(GoToMutationFile);
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public ObservableCollection<TestRunDocument> Mutations { get; set; }

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

                var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(config));

                foreach (var mutationDocument in mutationDocuments)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        Mutations.Add(new TestRunDocument
                        {
                            Document = mutationDocument,
                            Status = TestRunDocument.TestRunStatusEnum.Waiting
                        })));
                }
            });
        }

        public void Initialize(DTE dte)
        {
            _dte = dte;
            _solutionPath = dte.Solution.FullName;
        }

        private void GoToMutationFile(TestRunDocument obj)
        {
            var projItem = _dte.Solution.FindProjectItem(obj.Document.FilePath);
            var isOpen = projItem.IsOpen[EnvDTE.Constants.vsViewKindPrimary];
            var window = _dte.OpenFile(EnvDTE.Constants.vsViewKindPrimary, obj.Document.FilePath);
            window.Visible = true;
        }
    }
}
