using System.Windows.Controls;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView.
    /// </summary>
    public partial class MutationDocumentDetailsView : TabItem
    {
        public MutationDocumentDetailsView(MutationDocument document, MutationConfig config)
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
