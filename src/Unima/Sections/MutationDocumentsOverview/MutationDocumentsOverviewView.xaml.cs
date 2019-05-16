using System.Windows.Controls;
using Unima.Core.Config;

namespace Unima.Sections.MutationDocumentsOverview
{
    /// <summary>
    /// Interaction logic for MutationOverviewView
    /// </summary>
    public partial class MutationDocumentsOverviewView : TabItem
    {
        public MutationDocumentsOverviewView(UnimaConfig config)
        {
            InitializeComponent();

            var viewModel = DataContext as MutationDocumentsOverviewViewModel;
            viewModel.Initialize(config);
        }
    }
}
