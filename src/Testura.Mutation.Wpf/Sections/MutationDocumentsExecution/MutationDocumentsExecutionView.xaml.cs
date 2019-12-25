using System.Collections.Generic;
using System.Windows.Controls;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Execution.Report.Testura;

namespace Testura.Mutation.Wpf.Sections.MutationDocumentsExecution
{
    /// <summary>
    /// Interaction logic for TestRunView.
    /// </summary>
    public partial class MutationDocumentsExecutionView : TabItem
    {
        public MutationDocumentsExecutionView(IReadOnlyList<MutationDocument> documents, TesturaMutationConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetDocuments(documents, config);
        }

        public MutationDocumentsExecutionView(TesturaMutationReport report)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDocumentsExecutionViewModel;
            dataContext?.SetReport(report);
        }
    }
}
