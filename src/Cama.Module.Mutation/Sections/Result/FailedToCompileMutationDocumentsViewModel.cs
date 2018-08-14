using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cama.Core.Models.Mutation;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Result
{
    public class FailedToCompileMutationDocumentsViewModel : BindableBase, INotifyPropertyChanged
    {
        public FailedToCompileMutationDocumentsViewModel()
        {
            MutantsFailedToCompile = new ObservableCollection<MutationDocumentResult>();
        }

        public ObservableCollection<MutationDocumentResult> MutantsFailedToCompile { get; set; }

        public void InitializeMutants(IList<MutationDocumentResult> mutantsFailedToCompile)
        {
            MutantsFailedToCompile = new ObservableCollection<MutationDocumentResult>(mutantsFailedToCompile);
        }
    }
}
