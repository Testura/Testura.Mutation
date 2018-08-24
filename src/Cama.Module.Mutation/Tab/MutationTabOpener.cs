using System.Collections.Generic;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using Cama.Infrastructure.Tabs;
using Cama.Module.Mutation.Sections.Details;
using Cama.Module.Mutation.Sections.Overview;
using Cama.Module.Mutation.Sections.Result;
using Cama.Module.Mutation.Sections.TestRun;

namespace Cama.Module.Mutation.Tab
{
    public class MutationTabOpener : IMutationModuleTabOpener
    {
        private readonly IMainTabContainer _mainTabContainer;

        public MutationTabOpener(IMainTabContainer mainTabContainer)
        {
            _mainTabContainer = mainTabContainer;
        }

        public void OpenOverviewTab(CamaRunConfig config)
        {
            _mainTabContainer.RemoveAllTabs();
            _mainTabContainer.AddTab(new MutationOverviewView(config));
        }

        public void OpenDocumentDetailsTab(MutatedDocument document, CamaRunConfig config)
        {
            _mainTabContainer.AddTab(new MutationDetailsView(document, config));
        }

        public void OpenTestRunTab(IList<MutatedDocument> documents, CamaRunConfig config)
        {
            _mainTabContainer.AddTab(new TestRunView(documents, config));
        }

        public void OpenDocumentResultTab(MutationDocumentResult result)
        {
            _mainTabContainer.AddTab(new MutationDocumentTestResultView(result));
        }

        public void OpenFileDetailsTab(MFile file, CamaRunConfig config)
        {
            _mainTabContainer.AddTab(new FileDetailsView(file, config));
        }

        public void OpenFaildToCompileTab(IList<MutationDocumentResult> mutantsFailedToCompile)
        {
            _mainTabContainer.AddTab(new FailedToCompileMutationDocumentsView(mutantsFailedToCompile));
        }

        public void OpenAllMutationResultTab(List<MutationDocumentResult> completedMutations)
        {
            _mainTabContainer.AddTab(new AllCompletedMutationDocumentTestResultView(completedMutations));
        }
    }
}
