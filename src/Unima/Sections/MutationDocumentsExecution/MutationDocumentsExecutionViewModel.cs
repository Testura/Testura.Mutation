using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using MediatR;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Application.Commands.Mutation.ExecuteMutations;
using Unima.Application.Commands.Report.Creator;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Execution.Report;
using Unima.Core.Execution.Report.Html;
using Unima.Core.Execution.Report.Markdown;
using Unima.Core.Execution.Report.Summary;
using Unima.Core.Execution.Report.Trx;
using Unima.Core.Execution.Report.Unima;
using Unima.Helpers;
using Unima.Helpers.Displayers;
using Unima.Helpers.Openers.Tabs;
using Unima.Models;
using Unima.Wpf.Shared.Models;

namespace Unima.Sections.MutationDocumentsExecution
{
    public class MutationDocumentsExecutionViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly FilePicker _filePicker;
        private UnimaConfig _config;
        private IList<MutationDocumentResult> _latestResult;

        public MutationDocumentsExecutionViewModel(
            IMediator mediator,
            IMutationModuleTabOpener mutationModuleTabOpener,
            FilePicker filePicker)
        {
            MutationDocumentsExecutionResults = new MutationDocumentsExecutionResultModel();
            _mediator = mediator;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _filePicker = filePicker;
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

        public void SetDocuments(IReadOnlyList<MutationDocument> documents, UnimaConfig config)
        {
            _config = config;
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.Waiting }));
            MutationCount = documents.Count;
            MutationsInQueueCount = MutationCount;
        }

        public void SetReport(UnimaReport report)
        {
            TestNotStarted = false;
            MutationCount = report.TotalNumberOfMutations;
            MutationDocumentsExecutionResults.AddResult(report.Mutations);
        }

        private async void ExecuteMutationDocuments()
        {
            TestNotStarted = false;
            _latestResult = await _mediator.Send(
                new ExecuteMutationsCommand(
                    _config,
                    RunningDocuments.Select(r => r.Document).ToList(),
                    MutationDocumentStarted,
                    MutationDocumentCompleted));
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
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => RunningDocuments.Remove(runDocument)));
                }

                MutationDocumentsExecutionResults.AddResult(result);
            }
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }

        private async void SaveReportAsync()
        {
            if (_latestResult == null)
            {
                CommonDialogDisplayer.ShowErrorDialog("Nothing so save", "Please run before saving");
            }

            var savePath = _filePicker.PickDirectory();

            if (string.IsNullOrEmpty(savePath))
            {
                return;
            }

            var trxSavePath = Path.Combine(savePath, "result.trx");
            var reports = new List<ReportCreator>
            {
                new TrxReportCreator(trxSavePath),
                new MarkdownReportCreator(Path.ChangeExtension(trxSavePath, ".md")),
                new UnimaReportCreator(Path.ChangeExtension(trxSavePath, ".unima")),
                new HtmlOnlyBodyReportCreator(Path.ChangeExtension(trxSavePath, ".html")),
                new TextSummaryReportCreator(Path.ChangeExtension(trxSavePath, ".txt"))
            };

            await _mediator.Send(new CreateReportCommand(_latestResult, reports, TimeSpan.Zero));
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