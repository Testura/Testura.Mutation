using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using MediatR;
using Microsoft.VisualStudio.PlatformUI;
using Prism.Mvvm;
using Testura.Mutation.Application.Commands.Mutation.CreateMutations;
using Testura.Mutation.Application.Commands.Mutation.ExecuteMutations;
using Testura.Mutation.Application.Commands.Project.OpenProject;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.VsExtension.MutationHighlight;
using Testura.Mutation.VsExtension.Services;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly EnvironmentService _environmentService;
        private readonly ConfigService _configService;
        private readonly IMediator _mediator;
        private readonly MutationCodeHighlightHandler _mutationCodeHighlightHandler;

        private List<MutationDocumentFilterItem> _filterItems;
        private MutationConfig _config;
        private bool _showhighlight;
        private CancellationTokenSource _tokenSource;
        private IList<MutationDocumentResult> _mutationRunResult;

        public MutationExplorerWindowViewModel(
            EnvironmentService environmentService,
            ConfigService configService,
            MutationCodeHighlightHandler mutationCodeHighlightHandler,
            IMediator mediator)
        {
            _environmentService = environmentService;
            _configService = configService;
            _mediator = mediator;
            _mutationCodeHighlightHandler = mutationCodeHighlightHandler;
            _filterItems = new List<MutationDocumentFilterItem>();
            _mutationRunResult = new List<MutationDocumentResult>();

            Mutations = new ObservableCollection<TestRunDocument>();

            RunMutationsCommand = new DelegateCommand(RunMutations);
            MutationSelectedCommand = new DelegateCommand<TestRunDocument>(UpdateSelectedMutation);

            GoToMutationCommand = new DelegateCommand<TestRunDocument>(
                mutation => _environmentService.GoToLine(mutation.Document.FilePath, mutation.Document.MutationDetails.Location.GetLineNumber()));

            HighlightChangedCommand = new DelegateCommand<bool>(HightlightChanged);
            ToggleMutation = new DelegateCommand(() => IsMutationVisible = !IsMutationVisible);

            StopCommand = new DelegateCommand(() =>
            {
                IsStopButtonEnabled = false;
                _tokenSource.Cancel();
            });
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public DelegateCommand ToggleMutation { get; set; }

        public DelegateCommand<TestRunDocument> GoToMutationCommand { get; set; }

        public DelegateCommand<bool> HighlightChangedCommand { get; set; }

        public DelegateCommand StopCommand { get; set; }

        public ObservableCollection<TestRunDocument> Mutations { get; set; }

        public bool IsMutationVisible { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public SideBySideDiffModel Diff { get; private set; }

        public bool IsLoadingVisible { get; set; }

        public bool IsRunButtonEnabled { get; set; }

        public bool IsStopButtonEnabled { get; set; }

        public string LoadingMessage { get; set; }

        public void CreateMutations()
        {
            var baseFileConfig = _configService.GetBaseFileConfig();

            Mutations.Clear();
            ShowLoading("Loading mutations..");

            baseFileConfig.Filter = new MutationDocumentFilter { FilterItems = _filterItems ?? new List<MutationDocumentFilterItem>() };

            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    _config = await _mediator.Send(new OpenProjectCommand(baseFileConfig), _tokenSource.Token);

                    if (_tokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(_config), _tokenSource.Token);

                    await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                    foreach (var mutationDocument in mutationDocuments)
                    {
                        Mutations.Add(new TestRunDocument
                        {
                            Document = mutationDocument,
                            Status = TestRunDocument.TestRunStatusEnum.Waiting
                        });
                    }
                }
                catch (Exception)
                {
                    _environmentService.UserNotificationService.ShowError(
                        "Failed to create mutations. Check output log for more information.");
                }
                finally
                {
                    HideLoading();
                }
            });
        }

        public void Initialize(IEnumerable<MutationDocumentFilterItem> filterItems = null)
        {
            IsStopButtonEnabled = false;
            _tokenSource = new CancellationTokenSource();

            if (!_configService.ConfigExist())
            {
                return;
            }

            if (filterItems == null || !filterItems.Any())
            {
                var result = _environmentService.UserNotificationService.Confirm("You have not selected any file(s) or line(s) so we will mutate the whole project. Is this okay?");
                if (!result)
                {
                    _environmentService.CloseActiveWindow();
                    return;
                }
            }

            _filterItems = new List<MutationDocumentFilterItem>(
                filterItems ?? new List<MutationDocumentFilterItem>());

            CreateMutations();
        }

        public void Close()
        {
            _mutationCodeHighlightHandler.ClearHighlights();
            _tokenSource.Cancel();
        }

        private void RunMutations()
        {
            _tokenSource = new CancellationTokenSource();

            foreach (var testRunDocument in Mutations)
            {
                testRunDocument.Status = TestRunDocument.TestRunStatusEnum.Waiting;
                testRunDocument.InfoText = string.Empty;
            }

            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    IsStopButtonEnabled = true;
                    ShowLoading("Running mutations");

                    _mutationRunResult = await _mediator.Send(
                        new ExecuteMutationsCommand(
                            _config,
                            Mutations.Select(r => r.Document).ToList(),
                            MutationDocumentStarted,
                            MutationDocumentCompleted),
                        _tokenSource.Token);

                    UpdateHighlightedMutations();
                }
                catch (Exception)
                {
                    _environmentService.UserNotificationService.ShowError(
                        "Failed run mutations. Please check output for more information. ");
                }
                finally
                {
                    IsStopButtonEnabled = false;
                    HideLoading();
                }
            });
        }

        private void MutationDocumentCompleted(MutationDocumentResult result)
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                var runDocument = Mutations.FirstOrDefault(r => r.Document.Id == result.Id);

                if (runDocument != null)
                {
                    if (result.CompilationResult != null && !result.CompilationResult.IsSuccess)
                    {
                        runDocument.Status = TestRunDocument.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = "Failed to compile.";
                        return;
                    }

                    if (result.UnexpectedError != null)
                    {
                        runDocument.Status = TestRunDocument.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = result.UnexpectedError;
                        return;
                    }

                    runDocument.Status = result.Survived
                        ? TestRunDocument.TestRunStatusEnum.CompletedWithFailure
                        : TestRunDocument.TestRunStatusEnum.CompletedWithSuccess;
                    runDocument.InfoText = $"{result.FailedTests.Count} of {result.TestsRunCount} tests failed";
                }
            });
        }

        private void MutationDocumentStarted(MutationDocument mutationDocument)
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                var testRunDocument = Mutations.FirstOrDefault(r => r.Document == mutationDocument);
                if (testRunDocument != null)
                {
                    testRunDocument.Status = TestRunDocument.TestRunStatusEnum.Running;
                }
            });
        }

        private void UpdateSelectedMutation(TestRunDocument obj)
        {
            CodeBeforeMutation = obj.Document.MutationDetails.FullOrginal.ToFullString();
            CodeAfterMutation = obj.Document.MutationDetails.FullMutation.ToFullString();
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(CodeBeforeMutation, CodeAfterMutation);
        }

        private void ShowLoading(string message)
        {
            IsLoadingVisible = true;
            IsRunButtonEnabled = false;
            LoadingMessage = message;
        }

        private void HideLoading()
        {
            IsLoadingVisible = false;
            IsRunButtonEnabled = true;
        }

        private void HightlightChanged(bool isChecked)
        {
            _showhighlight = isChecked;

            if (_mutationRunResult.Any(m => m.Survived) && !IsLoadingVisible)
            {
                UpdateHighlightedMutations();
            }
        }

        private void UpdateHighlightedMutations()
        {
            if (_showhighlight)
            {
                var survivedMutations =
                    Mutations.Where(m => _mutationRunResult.Any(r => r.Id == m.Document.Id && r.Survived));
                _mutationCodeHighlightHandler.UpdateMutationHighlightList(survivedMutations);
                return;
            }

            _mutationCodeHighlightHandler.ClearHighlights();
        }
    }
}
