using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using Unima.VsExtension.MutationHighlight;
using Unima.VsExtension.Sections.Config;
using Unima.VsExtension.Wrappers;
using Unima.Wpf.Shared.Models;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    public class MutationExplorerWindowViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly EnvironmentWrapper _environmentWrapper;
        private readonly IMediator _mediator;
        private readonly MutationCodeHighlightHandler _mutationCodeHighlightHandler;
        private List<MutationDocumentFilterItem> _filterItems;
        private UnimaConfig _config;

        public MutationExplorerWindowViewModel(EnvironmentWrapper environmentWrapper, IMediator mediator, MutationCodeHighlightHandler mutationCodeHighlightHandler)
        {
            _environmentWrapper = environmentWrapper;
            _mediator = mediator;
            _mutationCodeHighlightHandler = mutationCodeHighlightHandler;
            _filterItems = new List<MutationDocumentFilterItem>();
            Mutations = new ObservableCollection<TestRunDocument>();
            RunMutationsCommand = new DelegateCommand(RunMutations);
            MutationSelectedCommand = new DelegateCommand<TestRunDocument>(UpdateSelectedMutation);
            GoToMutationCommand = new DelegateCommand<TestRunDocument>(GoToMutationFile);

            ToggleMutation = new DelegateCommand(() => IsMutationVisible = !IsMutationVisible);
        }

        public DelegateCommand RunMutationsCommand { get; set; }

        public DelegateCommand<TestRunDocument> MutationSelectedCommand { get; set; }

        public DelegateCommand ToggleMutation { get; set; }

        public DelegateCommand<TestRunDocument> GoToMutationCommand { get; set; }

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
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                Mutations.Clear();
                StartLoading("Loading mutations..");

                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var baseConfig = JsonConvert.DeserializeObject<UnimaFileConfig>(
                    File.ReadAllText(Path.Combine(Path.GetDirectoryName(_environmentWrapper.Dte.Solution.FullName), UnimaVsExtensionPackage.BaseConfigName)));

                await TaskScheduler.Default;

                try
                {
                    baseConfig.Filter = new MutationDocumentFilter { FilterItems = _filterItems ?? new List<MutationDocumentFilterItem>() };
                    _config = await _mediator.Send(new OpenProjectCommand(baseConfig));

                    var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(_config));

                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    foreach (var mutationDocument in mutationDocuments)
                    {
                        Mutations.Add(new TestRunDocument
                        {
                            Document = mutationDocument,
                            Status = TestRunDocument.TestRunStatusEnum.Waiting
                        });
                    }

                    _mutationCodeHighlightHandler.UpdateMutationHighlightList(new List<MutationHightlight>(Mutations.Select(m =>
                         new MutationHightlight
                         {
                             FilePath = m.Document.FilePath,
                             Line = int.Parse(m.Document.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries)[0]),
                             Start = m.Document.MutationDetails.Orginal.FullSpan.Start,
                             Length = m.Document.MutationDetails.Orginal.FullSpan.Length
                         })));
                }
                catch (Exception)
                {
                    _environmentWrapper.UserNotificationService.ShowError(
                        "Failed to create mutations. Check output log for more information.");
                }
                finally
                {
                    StopLoading();
                }
            });
        }

        public void Initialize(IEnumerable<MutationDocumentFilterItem> filterItems)
        {
            if (!VerifyConfigExist())
            {
                return;
            }

            _filterItems = new List<MutationDocumentFilterItem>(filterItems);
            CreateMutations();
        }

        public void Initialize()
        {
            if (!VerifyConfigExist())
            {
                return;
            }

            var result = _environmentWrapper.UserNotificationService.Confirm("You have not selected any file(s) so we will mutate the whole project. Is this okay?");

            if (result)
            {
                CreateMutations();
                return;
            }

            _environmentWrapper.JoinableTaskFactory.Run(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _environmentWrapper.Dte.ActiveWindow.Close();
            });
        }

        private void RunMutations()
        {
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    StartLoading("Running mutations");

                    var latestResult = await _mediator.Send(
                        new ExecuteMutationsCommand(
                            _config,
                            Mutations.Select(r => r.Document).ToList(),
                            MutationDocumentStarted,
                            MutationDocumentCompleted));
                }
                catch (Exception)
                {
                    _environmentWrapper.UserNotificationService.ShowError(
                        "Failed run mutations. Please check output for more information. ");
                }
                finally
                {
                    StopLoading();
                }
            });
        }

        private void MutationDocumentCompleted(MutationDocumentResult result)
        {
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var runDocument = Mutations.FirstOrDefault(r => r.Document.Id == result.Id);

                if (runDocument != null)
                {
                    runDocument.Status = result.Survived
                        ? TestRunDocument.TestRunStatusEnum.CompletedWithFailure
                        : TestRunDocument.TestRunStatusEnum.CompletedWithSuccess;

                    if (!result.CompilationResult.IsSuccess)
                    {
                        runDocument.InfoText = "Failed to compile.";
                        return;
                    }

                    runDocument.InfoText = $"{result.FailedTests.Count} of {result.TestsRunCount} tests failed";
                }
            });
        }

        private void MutationDocumentStarted(MutationDocument mutationDocument)
        {
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

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

        private void GoToMutationFile(TestRunDocument obj)
        {
            _environmentWrapper.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var window = _environmentWrapper.Dte.OpenFile(Constants.vsViewKindPrimary, obj.Document.FilePath);
                var line = obj.Document.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries);
                window.Visible = true;

                ((TextSelection)window.Document.Selection).GotoLine(int.Parse(line[0]), true);
            });
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

        private bool VerifyConfigExist()
        {
            return _environmentWrapper.JoinableTaskFactory.Run(async () =>
            {
                await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (!File.Exists(Path.Combine(Path.GetDirectoryName(_environmentWrapper.Dte.Solution.FullName), UnimaVsExtensionPackage.BaseConfigName)))
                {
                    _environmentWrapper.UserNotificationService.ShowWarning(
                        "Could not find base config. Please configure unima before running any mutation(s).");

                    await _environmentWrapper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    _environmentWrapper.Dte.ActiveWindow.Close();

                    _environmentWrapper.OpenWindow<UnimaConfigWindow>();

                    return false;
                }

                return true;
            });
        }
    }
}
