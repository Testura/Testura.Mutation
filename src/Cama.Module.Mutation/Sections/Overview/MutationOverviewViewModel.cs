using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Cama.Core.Models.Mutation;
using Cama.Core.Services;
using Cama.Infrastructure;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Overview
{
    public class MutationOverviewViewModel : BindableBase
    {
        private readonly SomeService _someService;
        private readonly IMutationModuleTabOpener _tabOpener;
        private readonly ILoadingDisplayer _loadingDisplayer;

        public MutationOverviewViewModel(SomeService someService, IMutationModuleTabOpener tabOpener, ILoadingDisplayer loadingDisplayer)
        {
            _someService = someService;
            _tabOpener = tabOpener;
            _loadingDisplayer = loadingDisplayer;
            Documents = new ObservableCollection<DocumentRowModel>();
            CreateDocumentsCommand = new DelegateCommand(CreateDocuments);
            DocumentSelectedCommand = new DelegateCommand<DocumentRowModel>(DocumentSelected);
            RunAllMutationsCommand = new DelegateCommand(RunAllMutations);
        }

        public DelegateCommand CreateDocumentsCommand { get; set; }

        public DelegateCommand RunAllMutationsCommand { get; set; }

        public DelegateCommand<DocumentRowModel> DocumentSelectedCommand { get; set; }

        public ObservableCollection<DocumentRowModel> Documents { get; set; }

        public bool IsMutationDocumentsLoaded => Documents.Any();

        private async void CreateDocuments()
        {
            _loadingDisplayer.ShowLoading("Creating mutation documents");
            var result = await _someService.DoSomeWorkAsync(@"D:\Programmering\Testura\Testura.Code\Testura.Code.sln", "Testura.Code", "Testura.Code.Tests");
            foreach (var mutatedDocument in result)
            {
                Documents.Add(new DocumentRowModel { Document = mutatedDocument, Status = "Not run" });
            }

            _loadingDisplayer.HideLoading();
        }

        private void RunAllMutations()
        {
            _tabOpener.OpenTestRunTab(Documents.Select(d => d.Document).ToList());
        }

        private void DocumentSelected(DocumentRowModel documentRow)
        {
            _tabOpener.OpenDocumentDetailsTab(documentRow.Document);
        }
    }
}
