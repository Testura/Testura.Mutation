using System.Collections.ObjectModel;
using Cama.Core.Services;
using Cama.Module.Mutation.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Overview
{
    public class MutationOverviewViewModel : BindableBase
    {
        private readonly SomeService _someService;

        public MutationOverviewViewModel(SomeService someService)
        {
            _someService = someService;
            Documents = new ObservableCollection<DocumentRowModel>();
            CreateDocumentsCommand = new DelegateCommand(CreateDocuments);
        }

        public DelegateCommand CreateDocumentsCommand { get; set; }

        public ObservableCollection<DocumentRowModel> Documents { get; set; }

        private async void CreateDocuments()
        {
            var result = await _someService.DoSomeWorkAsync(@"D:\Programmering\Testura\Testura.Code\Testura.Code.sln", "Testura.Code", "Testura.Code.Tests");
            foreach (var mutatedDocument in result)
            {
                Documents.Add(new DocumentRowModel { Document = mutatedDocument, Status = "Not run" });
            }
        }
    }
}
