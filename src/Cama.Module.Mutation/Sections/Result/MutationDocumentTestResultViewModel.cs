using System.ComponentModel;
using Cama.Core.Mutation.Models;
using Cama.Core.Report.Cama;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Result
{
    public class MutationDocumentTestResultViewModel : BindableBase, INotifyPropertyChanged
    {
        public MutationDocumentTestResultViewModel()
        {
            ShowFullCodeCommand = new DelegateCommand<bool?>(ShowFullCode);
        }

        public MutationDocumentResult Result { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public string Title { get; set; }

        public SideBySideDiffModel Diff { get; private set; }

        public DelegateCommand<bool?> ShowFullCodeCommand { get; set; }

        public void SetMutationDocumentTestResult(MutationDocumentResult result)
        {
            Result = result;
            Title = $"Test results for {result.FileName} - {result.Location}";
            ShowFullCode(false);
        }

        private void ShowFullCode(bool? showFullCode)
        {
            CodeBeforeMutation = showFullCode.Value ? Result.FullOrginal : Result.Orginal;
            CodeAfterMutation = showFullCode.Value ? Result.FullMutation : Result.Mutation;
            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(CodeBeforeMutation, CodeAfterMutation);
        }
    }
}