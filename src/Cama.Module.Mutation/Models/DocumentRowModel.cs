using System.ComponentModel;
using Cama.Business.Mutation;
using Testura.Module.TestRunner.Result;

namespace Cama.Module.Mutation.Models
{
    public class DocumentRowModel : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public MutatedDocument Document { get; set; }

        public string Status { get; set; }

        public NUnitTestSuiteResult LatestResult { get; set; }
    }
}
