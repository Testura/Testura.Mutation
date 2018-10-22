using System.ComponentModel;
using Cama.Core;

namespace Cama.Models
{
    public class TestRunDocument : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public enum TestRunStatusEnum
        {
            Running,
            Waiting
        }

        public MutationDocument Document { get; set; }

        public TestRunStatusEnum Status { get; set; }
    }
}
