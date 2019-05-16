using System.Collections.Generic;
using System.Windows.Controls;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Execution.Report.Unima;

namespace Unima.Sections.MutationDocumentsExecution
{
    /// <summary>
    /// Interaction logic for TestRunView
    /// </summary>
    public partial class MutationDocumentsExecutionView : TabItem
    {
        public MutationDocumentsExecutionView(IReadOnlyList<MutationDocument> documents, UnimaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetDocuments(documents, config);
        }

        public MutationDocumentsExecutionView(UnimaReport report)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetReport(report);
        }
    }
}
