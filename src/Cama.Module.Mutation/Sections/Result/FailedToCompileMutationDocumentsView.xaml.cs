using System.Collections.Generic;
using System.Windows.Controls;
using Cama.Core.Models.Mutation;
using Cama.Core.Report.Cama;

namespace Cama.Module.Mutation.Sections.Result
{
    /// <summary>
    /// Interaction logic for FailedToCompileMutationDocumentsView.xaml
    /// </summary>
    public partial class FailedToCompileMutationDocumentsView : TabItem
    {
        public FailedToCompileMutationDocumentsView(IList<CamaReportMutationItem> mutantsFailedToCompile)
        {
            InitializeComponent();

            var viewModel = DataContext as FailedToCompileMutationDocumentsViewModel;
            viewModel.InitializeMutants(mutantsFailedToCompile);
        }
    }
}
