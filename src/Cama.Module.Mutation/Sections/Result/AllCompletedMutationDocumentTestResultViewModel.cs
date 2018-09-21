using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Cama.Core.Models.Mutation;
using Cama.Core.Report.Cama;
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
            CompletedMutations = new ObservableCollection<CamaReportMutationItem>();
            CompletedDocumentSelectedCommand = new DelegateCommand<CamaReportMutationItem>(OpenCompleteDocumentTab);
        }

        public ObservableCollection<CamaReportMutationItem> CompletedMutations { get; set; }

        public DelegateCommand<CamaReportMutationItem> CompletedDocumentSelectedCommand { get; set; }

        public void Initialize(IList<CamaReportMutationItem> completedMutations)
        {
            CompletedMutations = new ObservableCollection<CamaReportMutationItem>(completedMutations);
        }

        private void OpenCompleteDocumentTab(CamaReportMutationItem obj)
        {
            _mutationModuleTabOpener.OpenDocumentResultTab(obj);
        }
    }
}
