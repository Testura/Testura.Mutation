using System.Collections.Generic;
using System.Windows.Controls;
using Cama.Core;
using Cama.Core.Execution.Report.Cama;

namespace Cama.Sections.MutationDocumentsExecution
{
    /// <summary>
    /// Interaction logic for TestRunView
    /// </summary>
    public partial class MutationDocumentsExecutionView : TabItem
    {
        public MutationDocumentsExecutionView(IReadOnlyList<MutationDocument> documents, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetDocuments(documents, config);
        }

        public MutationDocumentsExecutionView(CamaReport report)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetReport(report);
        }
    }
}
