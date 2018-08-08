using System.ComponentModel;
using Cama.Core.Models.Mutation;
using Prism.Mvvm;

namespace Cama.Module.Mutation.Sections.Result
{
    public class MutationDocumentTestResultViewModel : BindableBase, INotifyPropertyChanged
    {
        public MutationDocumentResult Result { get; set; }

        public string CodeAfterMutation { get; set; }

        public string CodeBeforeMutation { get; set; }

        public string Title { get; set; }

        public void SetMutationDocumentTestResult(MutationDocumentResult result)
        {
            Result = result;
            Title = $"Test results for {result.Document.FileName}";
            CodeBeforeMutation = result.Document.Replacer.Orginal.ToFullString();
            CodeAfterMutation = result.Document.Replacer.Replace.ToFullString();
        }
    }
}