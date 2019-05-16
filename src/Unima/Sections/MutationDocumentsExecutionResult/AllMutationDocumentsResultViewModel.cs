using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Core;
using Unima.Helpers.Openers.Tabs;

namespace Unima.Sections.MutationDocumentsExecutionResult
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
            if (obj != null)
            {
                _mutationModuleTabOpener.OpenDocumentResultTab(obj);
            }
        }
    }
}
