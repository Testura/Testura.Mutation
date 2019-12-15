using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using EnvDTE;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Commands.Mutation.CreateMutations;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Unima.Core.Creator.Filter;
using Unima.Wpf.Shared.Models;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private DTE _dte;
        private List<string> _files;
        private JoinableTaskFactory _joinableTaskFactory;

        public MutationExplorerWindowViewModel(IMediator mediator)
        {
            _mediator = mediator;
            _files = new List<string>();
            Mutations = new ObservableCollection<TestRunDocument>();
            RunMutationsCommand = new DelegateCommand(() => Do());
            MutationSelectedCommand = new DelegateCommand<TestRunDocument>(GoToMutationFile);
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public ObservableCollection<TestRunDocument> Mutations { get; set; }

        public void Do()
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var baseConfig = JsonConvert.DeserializeObject<UnimaFileConfig>(
                    File.ReadAllText(Path.Combine(Path.GetDirectoryName(_dte.Solution.FullName), UnimaVsExtensionPackage.BaseConfigName)));

                baseConfig.Filter = CreateFilter();

                var config = await _mediator.Send(new OpenProjectCommand(baseConfig));

                var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(config));

                foreach (var mutationDocument in mutationDocuments)
                {
                    Mutations.Add(new TestRunDocument
                    {
                        Document = mutationDocument,
                        Status = TestRunDocument.TestRunStatusEnum.Waiting
                    });
                }
            });
        }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            _dte = dte;
            _joinableTaskFactory = joinableTaskFactory;
        }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory, IEnumerable<string> files)
        {
            _dte = dte;
            _joinableTaskFactory = joinableTaskFactory;
            _files = new List<string>(files);
        }

        private void GoToMutationFile(TestRunDocument obj)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var projItem = _dte.Solution.FindProjectItem(obj.Document.FilePath);
                var isOpen = projItem.IsOpen[Constants.vsViewKindPrimary];
                var window = _dte.OpenFile(Constants.vsViewKindPrimary, obj.Document.FilePath);
                window.Visible = true;
            });
        }

        private MutationDocumentFilter CreateFilter()
        {
            if (!_files.Any())
            {
                return null;
            }

            var mutationFilter = new MutationDocumentFilter { FilterItems = new List<MutationDocumentFilterItem>() };

            foreach (var file in _files)
            {
                mutationFilter.FilterItems.Add(new MutationDocumentFilterItem
                {
                    Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                    Resource = $"*/{file}"
                });
            }

            return mutationFilter;
        }
    }
}
