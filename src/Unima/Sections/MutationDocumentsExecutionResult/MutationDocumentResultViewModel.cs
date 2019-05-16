using System.ComponentModel;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Prism.Commands;
using Prism.Mvvm;
using Unima.Core;

namespace Unima.Sections.MutationDocumentsExecutionResult
{
    public class MutationDocumentResultViewModel : BindableBase, INotifyPropertyChanged
    {
        public MutationDocumentResultViewModel()
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