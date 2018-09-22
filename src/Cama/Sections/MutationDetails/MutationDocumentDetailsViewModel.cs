using System.Collections.Generic;
using System.ComponentModel;
using Cama.Core;
using Cama.Tabs;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.MutationDetails
{
    public class MutationDocumentDetailsViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _tabOpener;
        private MutationDocument _document;
        private CamaConfig _config;

        public MutationDocumentDetailsViewModel(IMutationModuleTabOpener tabOpener)
        {
            _tabOpener = tabOpener;
            ExecuteTestsCommand = new DelegateCommand(ExecuteTests);
            ShowFullCodeCommand = new DelegateCommand<bool?>(ShowFullCode);
        }

        public IList<string> UnitTests { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public string FileName { get; set; }

        public DelegateCommand ExecuteTestsCommand { get; set; }

        public DelegateCommand<bool?> ShowFullCodeCommand { get; set; }

        public SideBySideDiffModel Diff { get; private set; }

        public string Title { get; set; }

        public void Initialize(MutationDocument document, CamaConfig config)
        {
            _config = config;
            _document = document;
            FileName = document.FileName;
            Title = $"{document.FileName} - {document.MutationDetails.Location})";
            ShowFullCode(false);
        }

        private void ShowFullCode(bool? showFullCode)
        {
            CodeBeforeMutation = showFullCode.Value ? _document.MutationDetails.FullOrginal.ToFullString() : _document.MutationDetails.Orginal.ToFullString();
            CodeAfterMutation = showFullCode.Value ? _document.MutationDetails.FullMutation.ToFullString() : _document.MutationDetails.Mutation.ToFullString();
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(CodeBeforeMutation, CodeAfterMutation);
        }

        private void ExecuteTests()
        {
            _tabOpener.OpenTestRunTab(new List<MutationDocument> { _document }, _config);
        }
    }
}
