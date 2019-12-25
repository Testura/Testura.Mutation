using System.Collections.Generic;
using Testura.Mutation.Application;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Execution.Report.Testura;

namespace Testura.Mutation.Wpf.Helpers.Openers.Tabs
{
    public interface IMutationModuleTabOpener
    {
        void OpenOverviewTab(MutationConfig config);

        void OpenDocumentDetailsTab(MutationDocument document, MutationConfig config);

        void OpenTestRunTab(IReadOnlyList<MutationDocument> documents, MutationConfig config);

        void OpenTestRunTab(TesturaMutationReport report);

        void OpenDocumentResultTab(MutationDocumentResult result);

        void OpenFileDetailsTab(FileMutationsModel file, MutationConfig config);

        void OpenFaildToCompileTab(IEnumerable<MutationDocumentResult> mutantsFailedToCompile);

        void OpenAllMutationResultTab(IEnumerable<MutationDocumentResult> completedMutations);
    }
}
