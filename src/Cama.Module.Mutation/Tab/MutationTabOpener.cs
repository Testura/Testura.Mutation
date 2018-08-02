using System.Collections.Generic;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
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

        public void OpenOverviewTab(CamaConfig config)
        {
            _mainTabContainer.RemoveAllTabs();
            _mainTabContainer.AddTab(new MutationOverviewView(config));
        }

        public void OpenDocumentDetailsTab(MutatedDocument document)
        {
            _mainTabContainer.AddTab(new MutationDetailsView(document));
        }

        public void OpenTestRunTab(IList<MutatedDocument> documents)
        {
            _mainTabContainer.AddTab(new TestRunView(documents));
        }

        public void OpenDocumentResultTab(MutationDocumentResult result)
        {
            _mainTabContainer.AddTab(new MutationDocumentTestResultView(result));
        }

        public void OpenFileDetailsTab(MFile file)
        {
            _mainTabContainer.AddTab(new FileDetailsView(file));
        }
    }
}
