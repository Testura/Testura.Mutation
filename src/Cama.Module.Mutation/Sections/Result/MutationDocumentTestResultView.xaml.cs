using System.Windows.Controls;
using Cama.Core.Mutation.Models;
using Cama.Core.Report.Cama;

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

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == BeforeTxt) ? AfterTxt : BeforeTxt;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
    }
}
