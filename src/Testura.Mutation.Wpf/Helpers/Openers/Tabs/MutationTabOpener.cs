using System.Collections.Generic;
using Testura.Mutation.Application;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Execution.Report.Testura;
using Testura.Mutation.Sections.MutationDetails;
using Testura.Mutation.Sections.MutationDocumentsExecutionResult;
using Testura.Mutation.Sections.MutationDocumentsOverview;
using MutationDocumentsExecutionView = Testura.Mutation.Wpf.Sections.MutationDocumentsExecution.MutationDocumentsExecutionView;

namespace Testura.Mutation.Wpf.Helpers.Openers.Tabs
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab(TesturaMutationConfig config)
        {
            _mainTabContainer.RemoveAllTabs();
            _mainTabContainer.AddTab(new MutationDocumentsOverviewView(config));
        }

        public void OpenDocumentDetailsTab(MutationDocument document, TesturaMutationConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentDetailsView(document, config));
        }

        public void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, TesturaMutationConfig config)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(documents, config));
        }

        public void OpenTestRunTab(TesturaMutationReport report)
        {
            _mainTabContainer.AddTab(new MutationDocumentsExecutionView(report));
        }

        public void OpenDocumentResultTab(MutationDocumentResult result)
        {
            _mainTabContainer.AddTab(new MutationDocumentResultView(result));
        }

        public void OpenFileDetailsTab(FileMutationsModel file, TesturaMutationConfig config)
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
