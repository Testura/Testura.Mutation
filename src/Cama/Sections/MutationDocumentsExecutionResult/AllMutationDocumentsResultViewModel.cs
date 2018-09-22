using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cama.Core;
using Cama.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.MutationDocumentsExecutionResult
{
    public class AllMutationDocumentsResultViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public AllMutationDocumentsResultViewModel(IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            CompletedMutations = new ObservableCollection<MutationDocumentResult>();
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
        }

        public ObservableCollection<MutationDocumentResult> CompletedMutations { get; set; }

        public DelegateCommand<MutationDocumentResult> CompletedDocumentSelectedCommand { get; set; }

        public void Initialize(IEnumerable<MutationDocumentResult> completedMutations)
        {
            CompletedMutations = new ObservableCollection<MutationDocumentResult>(completedMutations);
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }
    }
}
