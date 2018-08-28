using System.Windows.Controls;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationDetailsView : TabItem
    {
        public MutationDetailsView(MutatedDocument document, CamaConfig config)
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
