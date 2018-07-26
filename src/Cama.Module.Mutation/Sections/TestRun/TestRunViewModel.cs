using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cama.Core.Models.Mutation;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Models;
using Cama.Module.Mutation.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.TestRun
{
    public class TestRunViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly TestRunnerService _testRunnerService;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public TestRunViewModel(TestRunnerService testRunnerService, IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _testRunnerService = testRunnerService;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            RunningDocuments = new ObservableCollection<TestRunDocument>();
            CompletedDocuments = new ObservableCollection<MutationDocumentResult>();
            RunCommand = new DelegateCommand(RunTestsAsync);
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
        }

        public ObservableCollection<TestRunDocument> RunningDocuments { get; set; }

        public ObservableCollection<MutationDocumentResult> CompletedDocuments { get; set; }

        public DelegateCommand RunCommand { get; set; }

        public DelegateCommand<MutationDocumentResult> CompletedDocumentSelectedCommand { get; set; }

        public void SetDocuments(IList<MutatedDocument> documents)
        {
            RunningDocuments.AddRange(documents.Select(d => new TestRunDocument { Document = d, Status = TestRunDocument.TestRunStatusEnum.InQueue }));
        }

        private async void RunTestsAsync()
        {
            var runs = RunningDocuments.Select((d) => new Task(async () =>
            {
                var testResult = await _testRunnerService.RunTestAsync(d);
                MoveDocumentToCompleted(d, testResult);
            }));

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
            }
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }
    }
}