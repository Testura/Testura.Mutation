using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Cama.Core;
using Cama.Core.Execution.Report.Cama;
using Cama.Helpers.Openers.Tabs;
using Cama.Models;
using Cama.Service.Commands;
using Cama.Service.Commands.Mutation.ExecuteMutations;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.MutationDocumentsExecution
{
    public class MutationDocumentsExecutionViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private CamaConfig _config;

        public MutationDocumentsExecutionViewModel(
            ICommandDispatcher commandDispatcher,
            IMutationModuleTabOpener mutationModuleTabOpener)
        {
            MutationDocumentsExecutionResults = new MutationDocumentsExecutionResultModel();
            _commandDispatcher = commandDispatcher;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            RunningDocuments = new ObservableCollection<TestRunDocument>();
            RunCommand = new DelegateCommand(ExecuteMutationDocuments);
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
            SaveReportCommand = new DelegateCommand(SaveReportAsync);
            FailedToCompileCommand = new DelegateCommand(FailedToCompile);
            SeeAllMutationsCommand = new DelegateCommand(SeeAllMutations);

            MutationStatistics = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Killed",
                    Values = new ChartValues<ObservableValue> { MutationDocumentsExecutionResults.MutationsKilledCount },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
                },
                new PieSeries
                {
                    Title = "Survived",
                    Values = new ChartValues<ObservableValue> { MutationDocumentsExecutionResults.MutationsSurvivedCount },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
                }
            };

            TestNotStarted = true;
        }

        public bool TestNotStarted { get; set; }

        public int MutationCount { get; set; }

        public int MutationsInQueueCount { get; set; }

        public SeriesCollection MutationStatistics { get; set; }

        public ObservableCollection<TestRunDocument> RunningDocuments { get; set; }

        public MutationDocumentsExecutionResultModel MutationDocumentsExecutionResults { get; set; }

        public DelegateCommand RunCommand { get; set; }

        public DelegateCommand<MutationDocumentResult> CompletedDocumentSelectedCommand { get; set; }

        public DelegateCommand SaveReportCommand { get; set; }

        public DelegateCommand FailedToCompileCommand { get; set; }

        public DelegateCommand SeeAllMutationsCommand { get; set; }

        public void SetDocuments(IReadOnlyList<MutationDocument> documents, CamaConfig config)
        {
            _config = config;
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.Waiting }));
            MutationCount = documents.Count;
            MutationsInQueueCount = MutationCount;
        }

        public void SetReport(CamaReport report)
        {
            TestNotStarted = false;
            MutationCount = report.TotalNumberOfMutations;
            MutationDocumentsExecutionResults.AddResult(report.Mutations);
        }

        private async void ExecuteMutationDocuments()
        {
            TestNotStarted = false;
            await _commandDispatcher.ExecuteCommandAsync(
                new ExecuteMutationsCommand(_config, RunningDocuments.Select(r => r.Document).ToList(), MutationDocumentStarted, MutationDocumentCompleted));
        }

        private void MutationDocumentStarted(MutationDocument mutationDocument)
        {
            var testRunDocument = RunningDocuments.FirstOrDefault(r => r.Document == mutationDocument);
            if (testRunDocument != null)
            {
                MutationsInQueueCount--;
                testRunDocument.Status = TestRunDocument.TestRunStatusEnum.Running;
            }
        }

        private void MutationDocumentCompleted(MutationDocumentResult result)
        {
            lock (RunningDocuments)
            {
                var runDocument = RunningDocuments.FirstOrDefault(r => r.Document.Id == result.Id);

                if (runDocument != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => RunningDocuments.Remove(runDocument)));
                }

                MutationDocumentsExecutionResults.AddResult(result);
            }
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }

        private void SaveReportAsync()
        {
            // Change to open a tab instead
            /*
            _loadingDisplayer.ShowLoading("Saving report..");
            await Task.Run(() => _saveReportService.SaveReport(SurvivedDocuments));
            _loadingDisplayer.HideLoading();
            */
        }

        private void FailedToCompile()
        {
            _mutationModuleTabOpener.OpenFaildToCompileTab(MutationDocumentsExecutionResults.CompletedMutationDocuments.Where(m => !m.CompilationResult.IsSuccess));
        }

        private void SeeAllMutations()
        {
            _mutationModuleTabOpener.OpenAllMutationResultTab(MutationDocumentsExecutionResults.CompletedMutationDocuments);
        }
    }
}