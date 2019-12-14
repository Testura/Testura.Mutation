using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Application;
using Unima.Application.Commands.Mutation.CreateMutations;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Exceptions;
using Unima.Helpers.Displayers;
using Unima.Helpers.Extensions;
using Unima.Helpers.Openers.Tabs;
using Unima.Wpf.Shared.Models;

namespace Unima.Sections.MutationDocumentsOverview
{
    public class MutationDocumentsOverviewViewModel : BindableBase
    {
        private readonly IMediator _mediator;
        private readonly IMutationModuleTabOpener _tabOpener;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private UnimaConfig _config;

        public MutationDocumentsOverviewViewModel(IMediator mediator, IMutationModuleTabOpener tabOpener, ILoadingDisplayer loadingDisplayer)
        {
            _mediator = mediator;
            _tabOpener = tabOpener;
            _loadingDisplayer = loadingDisplayer;
            Documents = new ObservableCollection<DocumentRowModel>();
            CreateDocumentsCommand = new DelegateCommand(CreateDocuments);
            FileSelectedCommand = new DelegateCommand<DocumentRowModel>(DocumentSelected);
            RunAllMutationsCommand = new DelegateCommand(RunAllMutations);
            MutationOperatorGridItems = new ObservableCollection<MutationOperatorGridItem>(Enum
                .GetValues(typeof(MutationOperators)).Cast<MutationOperators>().Select(m =>
                    new MutationOperatorGridItem
                    {
                        IsSelected = true,
                        MutationOperator = m,
                        Description = m.GetValue()
                    }));
        }

        public DelegateCommand CreateDocumentsCommand { get; set; }

        public DelegateCommand RunAllMutationsCommand { get; set; }

        public DelegateCommand<DocumentRowModel> FileSelectedCommand { get; set; }

        public ObservableCollection<DocumentRowModel> Documents { get; set; }

        public ObservableCollection<MutationOperatorGridItem> MutationOperatorGridItems { get; set; }

        public void Initialize(UnimaConfig config)
        {
            _config = config;
        }

        private async void CreateDocuments()
        {
            _loadingDisplayer.ShowLoading("Creating mutation documents..");

            try
            {
                var settings = MutationOperatorGridItems.Where(m => m.IsSelected).Select(m => m.MutationOperator);
                var command = new CreateMutationsCommand(_config); // TODO: settings.Select(MutationOperatorFactory.GetMutationOperator).ToList()

                var mutationDocuments = await Task.Run(() => _mediator.Send(command));
                var fileNames = mutationDocuments.Select(r => r.FileName).Distinct();

                foreach (var fileName in fileNames)
                {
                    Documents.Add(new DocumentRowModel
                    {
                        MFile = new FileMutationsModel(fileName, mutationDocuments.Where(m => m.FileName == fileName).ToList())
                    });
                }
            }
            catch (MutationDocumentException ex)
            {
                CommonDialogDisplayer.ShowErrorDialog("Failed to create mutations", ex.Message, ex.InnerException?.Message);
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }

        private void RunAllMutations()
        {
            _tabOpener.OpenTestRunTab(Documents.SelectMany(d => d.MFile.MutationDocuments).ToList(), _config);
        }

        private void DocumentSelected(DocumentRowModel documentRow)
        {
            _tabOpener.OpenFileDetailsTab(documentRow.MFile, _config);
        }
    }
}
