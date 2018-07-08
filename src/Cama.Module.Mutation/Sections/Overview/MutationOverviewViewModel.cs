using System.Collections.ObjectModel;
using Cama.Common.Tabs;
using Cama.Core.Services;
using Cama.Module.Mutation.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Overview
{
    public class MutationOverviewViewModel : BindableBase
    {
        private readonly SomeService _someService;
        private readonly IMutationModuleTabOpener _tabOpener;

        public MutationOverviewViewModel(SomeService someService, IMutationModuleTabOpener tabOpener)
        {
            _someService = someService;
            _tabOpener = tabOpener;
            Documents = new ObservableCollection<DocumentRowModel>();
            CreateDocumentsCommand = new DelegateCommand(CreateDocuments);
            DocumentSelectedCommand = new DelegateCommand<DocumentRowModel>(DocumentSelected);
        }

        public DelegateCommand CreateDocumentsCommand { get; set; }

        public DelegateCommand<DocumentRowModel> DocumentSelectedCommand { get; set; }

        public ObservableCollection<DocumentRowModel> Documents { get; set; }

        private async void CreateDocuments()
        {
            var result = await _someService.DoSomeWorkAsync(@"D:\Programmering\Testura\Testura.Code\Testura.Code.sln", "Testura.Code", "Testura.Code.Tests");
            foreach (var mutatedDocument in result)
            {
                Documents.Add(new DocumentRowModel { Document = mutatedDocument, Status = "Not run" });
            }
        }

        private void DocumentSelected(DocumentRowModel documentRow)
        {
            _tabOpener.OpenDocumentDetailsTab(documentRow.Document);
        }
    }
}
