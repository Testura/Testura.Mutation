using System.ComponentModel;
using Unima.Application;

namespace Unima.Models
{
    public class DocumentRowModel : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public FileMutationsModel MFile { get; set; }

        public int MutationCount => MFile.MutationDocuments.Count;
    }
}
