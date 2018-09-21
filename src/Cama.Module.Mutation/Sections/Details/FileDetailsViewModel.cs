using System.ComponentModel;
using Cama.Core.Config;
using Cama.Core.Mutation.Models;
using Cama.Infrastructure.Models;
using Cama.Infrastructure.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Details
{
    public class FileDetailsViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _tabOpener;
        private CamaConfig _config;


        public FileDetailsViewModel(IMutationModuleTabOpener tabOpener)
        {
            _tabOpener = tabOpener;
            ExecuteTestsCommand = new DelegateCommand(ExecuteTests);
            MutationSelectedCommand = new DelegateCommand<MutationDocument>(MutationSelected);
        }

        public string FileName { get; set; }

        public FileMutationsModel File { get; set; }

        public DelegateCommand ExecuteTestsCommand { get; set; }

        public DelegateCommand<MutationDocument> MutationSelectedCommand { get; set; }

        public void Initialize(FileMutationsModel file, CamaConfig config)
        {
            _config = config;
            File = file;
            FileName = File.FileName;
        }

        private void ExecuteTests()
        {
            _tabOpener.OpenTestRunTab(File.MutatedDocuments, _config);
        }

        private void MutationSelected(MutationDocument obj)
        {
            _tabOpener.OpenDocumentDetailsTab(obj, _config);
        }
    }
}
