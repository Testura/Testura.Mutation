using System.ComponentModel;
using Unima.Core;

namespace Unima.Wpf.Shared.Models
{
    public class TestRunDocument : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public enum TestRunStatusEnum
        {
            Running,
            Waiting,
            CompletedWithSuccess,
            CompletedWithFailure
        }

        public MutationDocument Document { get; set; }

        public TestRunStatusEnum Status { get; set; }

        public string InfoText { get; set; }
    }
}
