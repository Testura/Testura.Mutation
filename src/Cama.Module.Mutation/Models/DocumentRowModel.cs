using System.ComponentModel;
using Cama.Core.Models;
using Cama.Infrastructure.Models;

namespace Cama.Module.Mutation.Models
{
    public class DocumentRowModel : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public FileMutationsModel MFile { get; set; }

        public int MutationCount => MFile.MutatedDocuments.Count;
    }
}
