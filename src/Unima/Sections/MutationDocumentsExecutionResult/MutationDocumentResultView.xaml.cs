using System.Windows.Controls;
using Unima.Core;

namespace Unima.Sections.MutationDocumentsExecutionResult
{
    /// <summary>
    /// Interaction logic for TestRunView
    /// </summary>
    public partial class MutationDocumentResultView : TabItem
    {
        public MutationDocumentResultView(MutationDocumentResult result)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentResultViewModel;
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
