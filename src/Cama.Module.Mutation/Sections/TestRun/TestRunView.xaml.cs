using System.Collections.Generic;
using System.Windows.Controls;
using Cama.Core.Config;
using Cama.Core.Mutation.Models;
using Cama.Core.Report.Cama;

namespace Cama.Module.Mutation.Sections.TestRun
{
    /// <summary>
    /// Interaction logic for TestRunView
    /// </summary>
    public partial class TestRunView : TabItem
    {
        public TestRunView(IList<MutationDocument> documents, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as TestRunViewModel;
            dataContext?.SetDocuments(documents, config);
        }

        public TestRunView(CamaReport report)
        {
            InitializeComponent();

            var dataContext = DataContext as TestRunViewModel;
            dataContext?.SetReport(report);
        }
    }
}
