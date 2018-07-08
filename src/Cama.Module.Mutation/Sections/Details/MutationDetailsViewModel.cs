using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core.Services;
using Cama.Module.Mutation.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Details
{
    public class MutationDetailsViewModel : BindableBase
    {
        private readonly SomeService _someService;

        public MutationDetailsViewModel(SomeService someService)
        {
            _someService = someService;
            Documents = new ObservableCollection<DocumentRowModel>();
            StartCommand = new DelegateCommand(Start);
        }

        public DelegateCommand StartCommand { get; set; }

        public ObservableCollection<DocumentRowModel> Documents { get; set; }

        private async void Start()
        {

        }

        private async Task RunTestsAsync()
        {
            var testService = new TestRunnerService();
            var runs = Documents.Select((d) => new Task(async () =>
            {
                d.Status = "Running..";
                var testResult = await testService.RunTestsAsync(d.Document);

                if (testResult == null)
                {
                    d.Status = "Unexpected result...";
                    return;
                }

                if (testResult.IsSuccess)
                {
                    d.Status = "Failed..";
                }
                else
                {
                    d.Status = "Success";
                }

                d.LatestResult = testResult;
            }));

            var queue = new Queue<Task>(runs.Take(10));
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

            var i = 0;
            i++;
        }
    }
}
