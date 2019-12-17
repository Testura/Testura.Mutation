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
using Unima.Application.Commands.Mutation.ExecuteMutations;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Models;
using Unima.Core;
using Unima.Core.Config;
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
        private UnimaConfig _config;

        public MutationExplorerWindowViewModel(IMediator mediator)
        {
            _mediator = mediator;
            _files = new List<string>();
            Mutations = new ObservableCollection<TestRunDocument>();
            RunMutationsCommand = new DelegateCommand(RunMutations);
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

        public bool IsRunButtonEnabled { get; set; }

        public string LoadingMessage { get; set; }

        public void CreateMutations()
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                Mutations.Clear();
                StartLoading("Loading mutations..");

                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var baseConfig = JsonConvert.DeserializeObject<UnimaFileConfig>(
                    File.ReadAllText(Path.Combine(Path.GetDirectoryName(_dte.Solution.FullName), UnimaVsExtensionPackage.BaseConfigName)));

                baseConfig.Filter = CreateFilter();

                _config = await _mediator.Send(new OpenProjectCommand(baseConfig));

                var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(_config));

                foreach (var mutationDocument in mutationDocuments)
                {
                    Mutations.Add(new TestRunDocument
                    {
                        Document = mutationDocument,
                        Status = TestRunDocument.TestRunStatusEnum.Waiting
                    });
                }

                StopLoading();
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

        private void RunMutations()
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                StartLoading("Running mutations");

                var latestResult = await _mediator.Send(
                    new ExecuteMutationsCommand(
                        _config,
                        Mutations.Select(r => r.Document).ToList(),
                        MutationDocumentStarted,
                        MutationDocumentCompleted));

                StopLoading();
            });
        }

        private void MutationDocumentCompleted(MutationDocumentResult result)
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var runDocument = Mutations.FirstOrDefault(r => r.Document.Id == result.Id);

                if (runDocument != null)
                {
                    runDocument.Status = TestRunDocument.TestRunStatusEnum.Completed;
                }
            });
        }

        private void MutationDocumentStarted(MutationDocument mutationDocument)
        {
            _joinableTaskFactory.RunAsync(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();

                var testRunDocument = Mutations.FirstOrDefault(r => r.Document == mutationDocument);
                if (testRunDocument != null)
                {
                    testRunDocument.Status = TestRunDocument.TestRunStatusEnum.Running;
                }
            });
        }

        private void GoToMutationFile(TestRunDocument obj)
        {
            ShowFullCode(false, obj.Document);

            _joinableTaskFactory.RunAsync(async () =>
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

        private void StartLoading(string message)
        {
            IsLoadingVisible = true;
            IsRunButtonEnabled = false;
            LoadingMessage = message;
        }

        private void StopLoading()
        {
            IsLoadingVisible = false;
            IsRunButtonEnabled = true;
        }
    }
}
