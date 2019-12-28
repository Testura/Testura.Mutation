using System.Windows.Controls;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Sections.MutationDocumentsOverview
{
    /// <summary>
    /// Interaction logic for MutationOverviewView
    /// </summary>
    public partial class MutationDocumentsOverviewView : TabItem
    {
        public MutationDocumentsOverviewView(MutationConfig config)
        {
            InitializeComponent();

            var viewModel = DataContext as MutationDocumentsOverviewViewModel;
            viewModel.Initialize(config);
        }
    }
}
