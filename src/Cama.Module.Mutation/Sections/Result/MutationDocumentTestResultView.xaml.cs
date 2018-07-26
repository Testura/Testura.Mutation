using System.Collections.Generic;
using System.Windows.Controls;
using Cama.Core.Models.Mutation;
using Cama.Module.Mutation.Sections.TestRun;

namespace Cama.Module.Mutation.Sections.Result
{
    /// <summary>
    /// Interaction logic for TestRunView
    /// </summary>
    public partial class MutationDocumentTestResultView : TabItem
    {
        public MutationDocumentTestResultView(MutationDocumentResult result)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentTestResultViewModel;
            dataContext.SetMutationDocumentTestResult(result);
        }
    }
}
