using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cama.Core.Models.Mutation;
using Cama.Infrastructure.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Result
{
    public class AllCompletedMutationDocumentTestResultViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public AllCompletedMutationDocumentTestResultViewModel(IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            CompletedMutations = new ObservableCollection<MutationDocumentResult>();
            CompletedDocumentSelectedCommand = new DelegateCommand<MutationDocumentResult>(OpenCompleteDocumentTab);
        }

        public ObservableCollection<MutationDocumentResult> CompletedMutations { get; set; }

        public DelegateCommand<MutationDocumentResult> CompletedDocumentSelectedCommand { get; set; }

        public void Initialize(IList<MutationDocumentResult> completedMutations)
        {
            CompletedMutations = new ObservableCollection<MutationDocumentResult>(completedMutations);
        }

        private void OpenCompleteDocumentTab(MutationDocumentResult obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }
    }
}
