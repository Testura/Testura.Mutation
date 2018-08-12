using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core;
using Cama.Core.Extensions;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Mutation.Mutators;
using Cama.Core.Services;
using Cama.Infrastructure;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Models;
using log4net;
using log4net.Appender;
using log4net.Layout;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Overview
{
    public class MutationOverviewViewModel : BindableBase
    {
        private readonly SomeService _someService;
        private readonly IMutationModuleTabOpener _tabOpener;
        private readonly ILoadingDisplayer _loadingDisplayer;
        private CamaConfig _config;

        public MutationOverviewViewModel(SomeService someService, IMutationModuleTabOpener tabOpener, ILoadingDisplayer loadingDisplayer)
        {
            _someService = someService;
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

        public bool IsMutationDocumentsLoaded => Documents.Any();

        private async void CreateDocuments()
        {
            _loadingDisplayer.ShowLoading("Creating mutation documents..");
            var settings = MutationOperatorGridItems.Where(m => m.IsSelected).Select(m => m.MutationOperator);
            var result = await Task.Run(() => _someService.DoSomeWorkAsync(_config.ProjectBinPath, _config.SolutionPath, _config.MutationProjectNames, _config.TestProjectName, settings.Select(MutationOperatorFactory.GetMutationOperator).ToList()));
            foreach (var mutatedDocument in result)
            {
                Documents.Add(new DocumentRowModel { MFile = mutatedDocument });
            }

            _loadingDisplayer.HideLoading();
        }

        public void Initialize(CamaConfig config)
        {
            _config = config;
        }

        private void RunAllMutations()
        {
            _tabOpener.OpenTestRunTab(Documents.SelectMany(d => d.MFile.StatementsMutations).ToList(), _config);
        }

        private void DocumentSelected(DocumentRowModel documentRow)
        {
            _tabOpener.OpenFileDetailsTab(documentRow.MFile, _config);
        }
    }
}
