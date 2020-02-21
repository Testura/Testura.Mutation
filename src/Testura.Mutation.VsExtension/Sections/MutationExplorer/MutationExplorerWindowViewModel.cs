using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TestRunDocument = Testura.Mutation.VsExtension.Models.TestRunDocument;

namespace Testura.Mutation.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase
    {
        private readonly EnvironmentService _environmentService;
        private readonly ConfigService _configService;
        private readonly IMediator _mediator;

        private bool _isMutationVisible;
        private string _codeAfterMutation;
        private string _codeBeforeMutation;
        private SideBySideDiffModel _diff;
        private bool _isLoadingVisible;
        private bool _isRunButtonEnabled;
        private bool _isStopButtonEnabled;
        private string _loadingMessage;

        private List<MutationDocumentFilterItem> _filterItems;
        private MutationConfig _config;
        private bool _showhighlight;
        private CancellationTokenSource _tokenSource;

        public MutationExplorerWindowViewModel(
            EnvironmentService environmentService,
            ConfigService configService,
            IMediator mediator)
        {
            _environmentService = environmentService;
            _configService = configService;
            _mediator = mediator;
            _filterItems = new List<MutationDocumentFilterItem>();

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

            _showhighlight = true;
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public DelegateCommand ToggleMutation { get; set; }

        public DelegateCommand<TestRunDocument> GoToMutationCommand { get; set; }

        public DelegateCommand<bool> HighlightChangedCommand { get; set; }

        public DelegateCommand StopCommand { get; set; }

        public ObservableCollection<TestRunDocument> Mutations { get; set; }

        public bool IsMutationVisible
        {
            get => _isMutationVisible;
            set => SetProperty(ref _isMutationVisible, value);
        }

        public string CodeAfterMutation
        {
            get => _codeAfterMutation;
            set => SetProperty(ref _codeAfterMutation, value);
        }

        public string CodeBeforeMutation
        {
            get => _codeBeforeMutation;
            set => SetProperty(ref _codeBeforeMutation, value);
        }

        public SideBySideDiffModel Diff
        {
            get => _diff;
            set => SetProperty(ref _diff, value);
        }

        public bool IsLoadingVisible
        {
            get => _isLoadingVisible;
            set => SetProperty(ref _isLoadingVisible, value);
        }

        public bool IsRunButtonEnabled
        {
            get => _isRunButtonEnabled;
            set => SetProperty(ref _isRunButtonEnabled, value);
        }

        public bool IsStopButtonEnabled
        {
            get => _isStopButtonEnabled;
            set => SetProperty(ref _isStopButtonEnabled, value);
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }

        public void CreateMutations()
        {
            var baseFileConfig = _configService.GetBaseFileConfig();

            Mutations.Clear();
            ShowLoading("Creating mutations..");

            if (_filterItems != null)
            {
                if (baseFileConfig.Filter == null)
                {
                    baseFileConfig.Filter = new MutationDocumentFilter();
                }

                baseFileConfig.Filter.FilterItems.AddRange(_filterItems);
            }

            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    _config = await _mediator.Send(new OpenProjectCommand(baseFileConfig), _tokenSource.Token);

                    if (_tokenSource.Token.IsCancellationRequested)
                    {
                        IsStopButtonEnabled = false;
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

                    IsRunButtonEnabled = true;
                    UpdateHighlightedMutations();
                }
                catch (Exception)
                {
                    _environmentService.UserNotificationService.ShowError(
                        "Failed to create mutations. Check output log for more information.");
                }
                finally
                {
                    IsLoadingVisible = false;
                }
            });
        }

        public void Initialize(IEnumerable<MutationDocumentFilterItem> filterItems = null)
        {
            if (IsLoadingVisible)
            {
                _environmentService.UserNotificationService.ShowInfoBar<MutationExplorerWindow>("Please stop your current mutation execution before starting a new one.");
                return;
            }

            IsStopButtonEnabled = true;
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
            MutationCodeHighlightHandler.ClearHighlights();
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

                    await _mediator.Send(
                        new ExecuteMutationsCommand(
                            _config,
                            Mutations.Select(r => r.Document).ToList(),
                            MutationDocumentStarted,
                            MutationDocumentCompleted),
                        _tokenSource.Token);
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

                    UpdateHighlightedMutations();
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
                        ? TestRunDocument.TestRunStatusEnum.CompleteAndSurvived
                        : TestRunDocument.TestRunStatusEnum.CompleteAndKilled;

                    runDocument.InfoText = $"{result.FailedTests.Count} of {result.TestsRunCount} tests failed";
                    runDocument.InfoText += result.Survived ? " (mutation survived)" : " (mutation killed)";
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

        private void UpdateSelectedMutation(TestRunDocument testRunDocument)
        {
            CodeBeforeMutation = testRunDocument?.Document?.MutationDetails?.FullOrginal?.ToFullString() ?? string.Empty;
            CodeAfterMutation = testRunDocument?.Document?.MutationDetails?.FullMutation?.ToFullString() ?? string.Empty;
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

            if (!IsLoadingVisible)
            {
                UpdateHighlightedMutations();
            }
        }

        private void UpdateHighlightedMutations()
        {
            if (_showhighlight)
            {
                MutationCodeHighlightHandler.UpdateMutationHighlightList(Mutations);
                return;
            }

            MutationCodeHighlightHandler.ClearHighlights();
        }
    }
}
