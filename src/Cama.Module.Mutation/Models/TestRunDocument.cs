using System.ComponentModel;
using Cama.Core.Models.Mutation;

namespace Cama.Module.Mutation.Models
{
    public class TestRunDocument : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public enum TestRunStatusEnum { Running, Waiting }

        public MutatedDocument Document { get; set; }

        public TestRunStatusEnum Status { get; set; }
    }
}
