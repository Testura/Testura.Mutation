using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cama.Core.Models.Mutation;
using Cama.Core.Services;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Models;
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
        private CamaConfig _config;

        public TestRunViewModel(TestRunnerService testRunnerService, IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _testRunnerService = testRunnerService;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            RunningDocuments = new ObservableCollection<TestRunDocument>();
            CompletedDocuments = new ObservableCollection<MutationDocumentResult>();
            SurvivedDocuments = new ObservableCollection<MutationDocumentResult>();
            RunCommand = new DelegateCommand(RunTestsAsync);
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
            MutationScore = "0%";
            MutationsSurvivedCount = new ObservableValue(0);
            MutationsKilledCount = new ObservableValue(0);

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
        }

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

        public void SetDocuments(IList<MutatedDocument> documents, CamaConfig config)
        {
            _config = config;
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.Waiting }));
            MutationCount = documents.Count;
        }

        private async void RunTestsAsync()
        {
            var runs = RunningDocuments.Select((d) => new Task(async () =>
            {
                d.Status = TestRunDocument.TestRunStatusEnum.Running;
                var testResult = await _testRunnerService.RunTestAsync(d.Document, _config.TestProjectOutputPath);
                MoveDocumentToCompleted(d, testResult);
            }));

            MutationsInQueueCount = runs.Count();
            var queue = new Queue<Task>(runs);
            var waitList = new List<Task>();

            while (queue.Any())
            {
                if (waitList.Count > 10)
                {
                    var finishedTask = await Task.WhenAny(waitList.ToArray());
                    waitList.Remove(finishedTask);
                }
                else
                {
                    MutationsInQueueCount--;
                    var newOne = queue.Dequeue();
                    newOne.Start();
                    waitList.Add(newOne);
                }
            }
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
    }
}