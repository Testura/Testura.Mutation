using System.Windows.Controls;
using Cama.Core.Config;
using Cama.Core.Mutation.Models;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationDetailsView : TabItem
    {
        public MutationDetailsView(MutationDocument document, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDetailsViewModel;
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
