using System.Collections.Generic;
using Unima.Application;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Execution.Report.Unima;
using Unima.Sections.MutationDetails;
using Unima.Sections.MutationDocumentsExecutionResult;
using Unima.Sections.MutationDocumentsOverview;
using MutationDocumentsExecutionView = Unima.Sections.MutationDocumentsExecution.MutationDocumentsExecutionView;

namespace Unima.Helpers.Openers.Tabs
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab(UnimaConfig config)
        {
            _mainTabContainer.RemoveAllTabs();
            _mainTabContainer.AddTab(new MutationDocumentsOverviewView(config));
        }

        public void OpenDocumentDetailsTab(MutationDocument document, UnimaConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentDetailsView(document, config));
        }

        public void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, UnimaConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(documents, config));
        }

        public void OpenTestRunTab(UnimaReport report)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(report));
        }

        public void OpenDocumentResultTab(MutationDocumentResult result)
        {
            _mainTabContainer.AddTab(new MutationDocumentResultView(result));
        }

        public void OpenFileDetailsTab(FileMutationsModel file, UnimaConfig config)
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
