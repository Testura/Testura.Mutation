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
using Testura.Mutation.Application.Commands.Mutation.ExecuteMutations;
using Testura.Mutation.Application.Commands.Report.Creator;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Execution.Report;
using Testura.Mutation.Core.Execution.Report.Html;
using Testura.Mutation.Core.Execution.Report.Markdown;
using Testura.Mutation.Core.Execution.Report.Summary;
using Testura.Mutation.Core.Execution.Report.Testura;
using Testura.Mutation.Core.Execution.Report.Trx;
using Testura.Mutation.Helpers;
using Testura.Mutation.Helpers.Displayers;
using Testura.Mutation.Models;
using Testura.Mutation.Wpf.Helpers.Openers.Tabs;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.Wpf.Sections.MutationDocumentsExecution
{
    public class MutationDocumentsExecutionViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly FilePicker _filePicker;
        private TesturaMutationConfig _config;
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

        public void SetDocuments(IReadOnlyList<MutationDocument> documents, TesturaMutationConfig config)
        {
            _config = config;
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.Waiting }));
            MutationCount = documents.Count;
            MutationsInQueueCount = MutationCount;
        }

        public void SetReport(TesturaMutationReport report)
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
                new TesturaMutationReportCreator(Path.ChangeExtension(trxSavePath, ".Testura.Mutation")),
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