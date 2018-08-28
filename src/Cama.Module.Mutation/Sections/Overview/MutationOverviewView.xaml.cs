using System.Windows.Controls;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;

namespace Cama.Module.Mutation.Sections.Overview
{
    /// <summary>
    /// Interaction logic for MutationOverviewView
    /// </summary>
    public partial class MutationOverviewView : TabItem
    {
        public MutationOverviewView(CamaConfig config)
        {
            InitializeComponent();

            var viewModel = DataContext as MutationOverviewViewModel;
            viewModel.Initialize(config);
        }
    }
}
