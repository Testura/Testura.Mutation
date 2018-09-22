using System.Collections.Generic;
using Cama.Core;
using Cama.Core.Execution.Report.Cama;
using Cama.Sections.MutationDetails;
using Cama.Sections.MutationDocumentsExecutionResult;
using Cama.Sections.MutationDocumentsOverview;
using Cama.Service;
using Cama.Tabs;
using MutationDocumentsExecutionView = Cama.Sections.MutationDocumentsExecution.MutationDocumentsExecutionView;

namespace Cama.Tab
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab(CamaConfig config)
        {
            _mainTabContainer.RemoveAllTabs();
            _mainTabContainer.AddTab(new MutationDocumentsOverviewView(config));
        }

        public void OpenDocumentDetailsTab(MutationDocument document, CamaConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentDetailsView(document, config));
        }

        public void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, CamaConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(documents, config));
        }

        public void OpenTestRunTab(CamaReport report)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(report));
        }

        public void OpenDocumentResultTab(MutationDocumentResult result)
        {
            _mainTabContainer.AddTab(new MutationDocumentResultView(result));
        }

        public void OpenFileDetailsTab(FileMutationsModel file, CamaConfig config)
        {
            _mainTabContainer.AddTab(new MutationFileDetailsView(file, config));
        }

        public void OpenFaildToCompileTab(IEnumerable<MutationDocumentResult> mutantsFailedToCompile)
        {
            _mainTabContainer.AddTab(new FailedToCompileMutationDocumentsView(mutantsFailedToCompile));
        }

        public void OpenAllMutationResultTab(IEnumerable<MutationDocumentResult> completedMutations)
        {
            _mainTabContainer.AddTab(new AllMutationDocumentsResultView(completedMutations));
        }
    }
}
