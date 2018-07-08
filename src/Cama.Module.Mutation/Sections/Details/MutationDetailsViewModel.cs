using System.ComponentModel;
using System.Threading.Tasks;
using Cama.Core.Mutation;
using Microsoft.CodeAnalysis;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Details
{
    public class MutationDetailsViewModel : BindableBase, INotifyPropertyChanged
    {
        public MutationDetailsViewModel()
        {
        }

        public MutatedDocument Document { get; set; }

        public void Initialize(MutatedDocument document)
        {
            Document = document;
        }

        private async Task RunTestsAsync()
        {
            /*
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
            */
        }
    }
}
