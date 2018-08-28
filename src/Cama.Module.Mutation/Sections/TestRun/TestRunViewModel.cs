using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using Cama.Core.Services;
using Cama.Infrastructure;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Models;
using Cama.Module.Mutation.Services;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.TestRun
{
    public class TestRunViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly TestRunnerService _testRunnerService;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly SaveReportService _saveReportService;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private List<MutationDocumentResult> _mutantsFailedToCompile;
        private CamaRunConfig _config;

        public TestRunViewModel(TestRunnerService testRunnerService, IMutationModuleTabOpener mutationModuleTabOpener, SaveReportService saveReportService, ILoadingDisplayer loadingDisplayer)
        {
            _testRunnerService = testRunnerService;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _saveReportService = saveReportService;
            _loadingDisplayer = loadingDisplayer;
            RunningDocuments = new ObservableCollection<TestRunDocument>();
            CompletedDocuments = new ObservableCollection<MutationDocumentResult>();
            SurvivedDocuments = new ObservableCollection<MutationDocumentResult>();
            RunCommand = new DelegateCommand(RunTestsAsync);
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
            SaveReportCommand = new DelegateCommand(SaveReportAsync);
            FailedToCompileCommand = new DelegateCommand(FailedToCompile);
            SeeAllMutationsCommand = new DelegateCommand(SeeAllMutations);
            MutationScore = "0%";
            MutationsSurvivedCount = new ObservableValue(0);
            MutationsKilledCount = new ObservableValue(0);
            _mutantsFailedToCompile = new List<MutationDocumentResult>();

            MutationStatistics = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Survived",
                    Values = new ChartValues<ObservableValue> { MutationsSurvivedCount },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
                },
                new PieSeries
                {
                    Title = "Killed",
                    Values = new ChartValues<ObservableValue> { MutationsKilledCount },
                    DataLabels = true,
                    LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})"
                }
            };

            TestNotStarted = true;
        }

        public bool TestNotStarted { get; set; }

        public int MutationCount { get; set; }

        public int FinishedMutationsCount { get; set; }

        public int MutationsInQueueCount { get; set; }

        public ObservableValue MutationsSurvivedCount { get; set; }

        public ObservableValue MutationsKilledCount { get; set; }

        public int FailedToCompileMutationsCount { get; set; }

        public string MutationScore { get; set; }

        public SeriesCollection MutationStatistics { get; set; }

        public ObservableCollection<TestRunDocument> RunningDocuments { get; set; }

        public ObservableCollection<MutationDocumentResult> CompletedDocuments { get; set; }

        public ObservableCollection<MutationDocumentResult> SurvivedDocuments { get; set; }

        public DelegateCommand RunCommand { get; set; }

        public DelegateCommand<MutationDocumentResult> CompletedDocumentSelectedCommand { get; set; }

        public DelegateCommand SaveReportCommand { get; set; }

        public DelegateCommand FailedToCompileCommand { get; set; }

        public DelegateCommand SeeAllMutationsCommand { get; set; }

        public void SetDocuments(IList<MutatedDocument> documents, CamaRunConfig config)
        {
            _config = config;
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.Waiting }));
            MutationCount = documents.Count;
        }

        private async void RunTestsAsync()
        {
            TestNotStarted = false;
            var semaphoreSlim = new SemaphoreSlim(4, 4);

            await Task.WhenAll(RunningDocuments.Select((d) => Task.Run(async () =>
            {
                semaphoreSlim.Wait();
                MutationsInQueueCount--;

                d.Status = TestRunDocument.TestRunStatusEnum.Running;
                var testResult = await _testRunnerService.RunTestAsync(_config, d.Document);
                MoveDocumentToCompleted(d, testResult);

                semaphoreSlim.Release();
            })).ToArray());
        }

        private void MoveDocumentToCompleted(TestRunDocument runDocument, MutationDocumentResult result)
        {
            lock (RunningDocuments)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => RunningDocuments.Remove(runDocument)));
                Application.Current.Dispatcher.BeginInvoke(new Action(() => CompletedDocuments.Add(result)));
                FinishedMutationsCount++;

                if (!result.CompilerResult.IsSuccess)
                {
                    FailedToCompileMutationsCount++;
                    _mutantsFailedToCompile.Add(result);
                    return;
                }

                if (result.Survived)
                {
                    MutationsSurvivedCount.Value++;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => SurvivedDocuments.Add(result)));
                }
                else
                {
                    MutationsKilledCount.Value++;
                }

                MutationScore = $"{Math.Round((MutationsKilledCount.Value / (MutationsKilledCount.Value + MutationsSurvivedCount.Value)) * 100)}%";
            }
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }

        private async void SaveReportAsync()
        {
            _loadingDisplayer.ShowLoading("Saving report..");
            await Task.Run(() => _saveReportService.SaveReport(SurvivedDocuments));
            _loadingDisplayer.HideLoading();
        }

        private void FailedToCompile()
        {
            _mutationModuleTabOpener.OpenFaildToCompileTab(_mutantsFailedToCompile);
        }

        private void SeeAllMutations()
        {
            _mutationModuleTabOpener.OpenAllMutationResultTab(CompletedDocuments.ToList());
        }
    }
}