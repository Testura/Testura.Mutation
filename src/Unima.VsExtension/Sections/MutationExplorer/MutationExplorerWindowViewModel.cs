using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using EnvDTE;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using Prism.Mvvm;
using Unima.Application.Commands.Mutation.CreateMutations;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Unima.Core;
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
            /* RunMutationsCommand = new DelegateCommand(() => Do()); */
            MutationSelectedCommand = new DelegateCommand<TestRunDocument>(GoToMutationFile);
            ToggleMutation = new DelegateCommand(() => IsMutationVisible = !IsMutationVisible);
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public DelegateCommand ToggleMutation { get; set; }

        public ObservableCollection<TestRunDocument> Mutations { get; set; }

        public bool IsMutationVisible { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public SideBySideDiffModel Diff { get; private set; }

        public bool IsLoadingVisible { get; set; }

        public string LoadingMessage { get; set; }

        public void CreateMutations()
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                Mutations.Clear();
                IsLoadingVisible = true;
                LoadingMessage = "Loading mutations..";

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

                IsLoadingVisible = false;
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

            CreateMutations();
        }

        private void GoToMutationFile(TestRunDocument obj)
        {
            ShowFullCode(false, obj.Document);

            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var window = _dte.OpenFile(Constants.vsViewKindPrimary, obj.Document.FilePath);
                var line = obj.Document.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries);
                window.Visible = true;

                ((TextSelection)window.Document.Selection).GotoLine(int.Parse(line[0]), true);
            });
        }

        private void ShowFullCode(bool? showFullCode, MutationDocument mutationDocument)
        {
            CodeBeforeMutation = showFullCode.Value ? mutationDocument.MutationDetails.FullOrginal.ToFullString() : mutationDocument.MutationDetails.Orginal.ToFullString();
            CodeAfterMutation = showFullCode.Value ? mutationDocument.MutationDetails.FullMutation.ToFullString() : mutationDocument.MutationDetails.Mutation.ToFullString();
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(CodeBeforeMutation, CodeAfterMutation);
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
