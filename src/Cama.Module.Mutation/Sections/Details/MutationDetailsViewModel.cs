using System.Collections.Generic;
using System.ComponentModel;
using Cama.Core.Models.Mutation;
using Cama.Infrastructure.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Details
{
    public class MutationDetailsViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _tabOpener;
        private MutatedDocument _document;

        public MutationDetailsViewModel(IMutationModuleTabOpener tabOpener)
        {
            _tabOpener = tabOpener;
            ExecuteTestsCommand = new DelegateCommand(ExecuteTests);
        }

        public IList<string> UnitTests { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public string FileName { get; set; }

        public DelegateCommand ExecuteTestsCommand { get; set; }

        public void Initialize(MutatedDocument document)
        {
            _document = document;
            FileName = document.FileName;
            CodeBeforeMutation = document.Replacer.Orginal.ToFullString();
            CodeAfterMutation = document.Replacer.Replace.ToFullString();
            UnitTests = document.Tests;
        }

        private void ExecuteTests()
        {
            _tabOpener.OpenTestRunTab(new List<MutatedDocument> { _document });
        }
    }
}
