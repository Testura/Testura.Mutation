using System.ComponentModel;
using Cama.Core.Models;

namespace Cama.Module.Mutation.Models
{
    public class DocumentRowModel : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public MFile MFile { get; set; }

        public int MutationCount => MFile.StatementsMutations.Count;
    }
}
