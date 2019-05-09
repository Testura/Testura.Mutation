using System.Windows.Controls;
using Cama.Core.Config;

namespace Cama.Sections.MutationDocumentsOverview
{
    /// <summary>
    /// Interaction logic for MutationOverviewView
    /// </summary>
    public partial class MutationDocumentsOverviewView : TabItem
    {
        public MutationDocumentsOverviewView(CamaConfig config)
        {
            InitializeComponent();

            var viewModel = DataContext as MutationDocumentsOverviewViewModel;
            viewModel.Initialize(config);
        }
    }
}
