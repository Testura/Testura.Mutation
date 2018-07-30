using System.ComponentModel;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Infrastructure.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Details
{
    public class FileDetailsViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _tabOpener;


        public FileDetailsViewModel(IMutationModuleTabOpener tabOpener)
        {
            _tabOpener = tabOpener;
            ExecuteTestsCommand = new DelegateCommand(ExecuteTests);
            MutationSelectedCommand = new DelegateCommand<MutatedDocument>(MutationSelected);
        }

        public string FileName { get; set; }

        public MFile File { get; set; }

        public DelegateCommand ExecuteTestsCommand { get; set; }

        public DelegateCommand<MutatedDocument> MutationSelectedCommand { get; set; }

        public void Initialize(MFile file)
        {
            File = file;
            FileName = File.FileName;
        }

        private void ExecuteTests()
        {
            _tabOpener.OpenTestRunTab(File.StatementsMutations);
        }

        private void MutationSelected(MutatedDocument obj)
        {
            _tabOpener.OpenDocumentDetailsTab(obj);
        }
    }
}
