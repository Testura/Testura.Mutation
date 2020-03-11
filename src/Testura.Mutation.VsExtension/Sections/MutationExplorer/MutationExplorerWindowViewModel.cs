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
using Testura.Mutation.VsExtension.Models;
using Testura.Mutation.VsExtension.MutationHighlight;
using Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog;
using Testura.Mutation.VsExtension.Services;

namespace Testura.Mutation.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase
    {
        private readonly EnvironmentService _environmentService;
        private readonly ConfigService _configService;
        private readonly IMediator _mediator;

        private string _codeAfterMutation;
        private string _codeBeforeMutation;
        private SideBySideDiffModel _diff;

        private bool _showhighlight;
        private bool _isMutationVisible;
        private bool _isRunButtonEnabled;
        private bool _isStopButtonEnabled;
        private bool _isLoadingMutationsVisible;

        private MutationRunItem _selectedMutation;
        private List<MutationDocumentFilterItem> _filterItems;
        private MutationConfig _config;
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

            Mutations = new ObservableCollection<MutationRunItem>();

            RunMutationsCommand = new DelegateCommand(() => RunMutations(Mutations));
            MutationSelectedCommand = new DelegateCommand<MutationRunItem>(UpdateSelectedMutation);

            GoToMutationCommand = new DelegateCommand<MutationRunItem>(
                mutation => _environmentService.GoToLine(mutation.Document.FilePath, mutation.Document.MutationDetails.Location.GetLineNumber()));

            HighlightChangedCommand = new DelegateCommand<bool>(HightlightChanged);
            ToggleMutation = new DelegateCommand(() => IsMutationVisible = !IsMutationVisible);
            ShowMutationDetailsCommand = new DelegateCommand(ShowMutationDetails);
            RemoveKilledMutationsCommand = new DelegateCommand(RemovedKilledMutations);
            RunOnlySurvivingMutationsCommand = new DelegateCommand(RunOnlySurvivingMutations);

            StopCommand = new DelegateCommand(() =>
            {
                IsStopButtonEnabled = false;
                _tokenSource.Cancel();
            });

            _showhighlight = true;
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<MutationRunItem> MutationSelectedCommand { get; set; }

        public DelegateCommand ToggleMutation { get; set; }

        public DelegateCommand ShowMutationDetailsCommand { get; set; }

        public DelegateCommand<MutationRunItem> GoToMutationCommand { get; set; }

        public DelegateCommand<bool> HighlightChangedCommand { get; set; }

        public DelegateCommand StopCommand { get; set; }

        public DelegateCommand RemoveKilledMutationsCommand { get; set; }

        public DelegateCommand RunOnlySurvivingMutationsCommand { get; set; }

        public ObservableCollection<MutationRunItem> Mutations { get; set; }

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

        public bool IsLoadingMutationsVisible
        {
            get => _isLoadingMutationsVisible;
            set => SetProperty(ref _isLoadingMutationsVisible, value);
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

        public MutationRunItem SelectedMutation
        {
            get => _selectedMutation;
            set => SetProperty(ref _selectedMutation, value);
        }

        public void CreateMutations()
        {
            var baseFileConfig = _configService.GetBaseFileConfig();

            if (baseFileConfig == null)
            {
                return;
            }

            Mutations.Clear();
            IsLoadingMutationsVisible = true;
            IsStopButtonEnabled = true;

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
                        Mutations.Add(new MutationRunItem
                        {
                            Document = mutationDocument,
                            Status = MutationRunItem.TestRunStatusEnum.Waiting,
                            InfoText = "Not run"
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
                    IsStopButtonEnabled = false;
                    IsLoadingMutationsVisible = false;
                }
            });
        }

        public void Initialize(IEnumerable<MutationDocumentFilterItem> filterItems = null)
        {
            if (IsLoadingMutationsVisible)
            {
                _environmentService.UserNotificationService.ShowInfoBar<MutationExplorerWindow>("Please stop your current mutation execution before starting a new one.");
                return;
            }

            IsRunButtonEnabled = false;
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

        private void RunMutations(IEnumerable<MutationRunItem> mutations)
        {
            if (mutations == null || !mutations.Any())
            {
                return;
            }

            _tokenSource = new CancellationTokenSource();

            foreach (var mutation in mutations)
            {
                mutation.Status = MutationRunItem.TestRunStatusEnum.Waiting;
                mutation.InfoText = "Waiting..";
            }

            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    IsStopButtonEnabled = true;
                    IsRunButtonEnabled = false;

                    await _mediator.Send(
                        new ExecuteMutationsCommand(
                            _config,
                            mutations.Select(r => r.Document).ToList(),
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
                    IsRunButtonEnabled = true;
                    IsStopButtonEnabled = false;

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
                    runDocument.Result = result;

                    if (result.CompilationResult != null && !result.CompilationResult.IsSuccess)
                    {
                        runDocument.Status = MutationRunItem.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = "Failed to compile.";
                        return;
                    }

                    if (result.UnexpectedError != null)
                    {
                        runDocument.Status = MutationRunItem.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = result.UnexpectedError;
                        return;
                    }

                    runDocument.Status = result.Survived
                        ? MutationRunItem.TestRunStatusEnum.CompleteAndSurvived
                        : MutationRunItem.TestRunStatusEnum.CompleteAndKilled;

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
                    testRunDocument.Status = MutationRunItem.TestRunStatusEnum.Running;
                    testRunDocument.InfoText = "Running..";
                }
            });
        }

        private void UpdateSelectedMutation(MutationRunItem mutationRunItem)
        {
            SelectedMutation = mutationRunItem;

            CodeBeforeMutation = mutationRunItem?.Document?.MutationDetails?.Orginal?.ToFullString() ?? string.Empty;
            CodeAfterMutation = mutationRunItem?.Document?.MutationDetails?.Mutation?.ToFullString() ?? string.Empty;
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(CodeBeforeMutation, CodeAfterMutation);
        }

        private void HightlightChanged(bool isChecked)
        {
            _showhighlight = isChecked;
            UpdateHighlightedMutations();
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

        private void ShowMutationDetails()
        {
            if (_selectedMutation == null)
            {
                return;
            }

            var documentationControl = new MutationCodeHiglightInfoDialog(_selectedMutation);
            documentationControl.ShowDialog();
        }

        private void RemovedKilledMutations()
        {
            var index = 0;
            while (index < Mutations.Count)
            {
                if (Mutations[index].Result != null && !Mutations[index].Result.Survived)
                {
                    Mutations.RemoveAt(index);
                    continue;
                }

                index++;
            }

            UpdateHighlightedMutations();
        }

        private void RunOnlySurvivingMutations()
        {
            RunMutations(Mutations.Where(m => m.Status == MutationRunItem.TestRunStatusEnum.CompleteAndSurvived).ToList());
        }
    }
}
