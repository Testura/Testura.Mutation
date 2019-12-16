using System.Windows.Controls;
using Unima.Core;
using Unima.Core.Config;

namespace Unima.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView.
    /// </summary>
    public partial class MutationDocumentDetailsView : TabItem
    {
        public MutationDocumentDetailsView(MutationDocument document, UnimaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentDetailsViewModel;
            dataContext.Initialize(document, config);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == BeforeTxt) ? AfterTxt : BeforeTxt;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
    }
}
