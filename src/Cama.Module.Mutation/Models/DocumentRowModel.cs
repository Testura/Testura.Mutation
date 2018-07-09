using System.ComponentModel;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;

namespace Cama.Module.Mutation.Models
{
    public class DocumentRowModel : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public MutatedDocument Document { get; set; }

        public string Status { get; set; }

        public TestSuiteResult LatestResult { get; set; }
    }
}
