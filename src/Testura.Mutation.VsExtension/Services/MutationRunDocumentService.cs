using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnvDTE;
using Testura.Mutation.Core;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.Services
{
    public class MutationRunDocumentService
    {
        private readonly EnvironmentService _environmentService;
        private readonly ObservableCollection<MutationRunItem> _mutations;

        public MutationRunDocumentService(EnvironmentService environmentService, ObservableCollection<MutationRunItem> mutations)
        {
            _environmentService = environmentService;
            _mutations = mutations;
        }

        public void AddMutationDocuments(IList<MutationDocument> mutationDocuments)
        {
            foreach (var mutationDocument in mutationDocuments)
            {
                _mutations.Add(new MutationRunItem
                {
                    Document = mutationDocument,
                    Status = MutationRunItem.TestRunStatusEnum.Waiting,
                    InfoText = "Not run"
                });
            }
        }

        public void RemovedKilledMutations()
        {
            var index = 0;
            while (index < _mutations.Count)
            {
                if (_mutations[index].Result != null && !_mutations[index].Result.Survived)
                {
                    _mutations.RemoveAt(index);
                    continue;
                }

                index++;
            }
        }

        public void MutationDocumentStarted(MutationDocument mutationDocument)
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                var testRunDocument = _mutations.FirstOrDefault(r => r.Document == mutationDocument);
                if (testRunDocument != null)
                {
                    testRunDocument.Status = MutationRunItem.TestRunStatusEnum.Running;
                    testRunDocument.InfoText = "Running..";
                }
            });
        }

        public void MutationDocumentCompleted(MutationDocumentResult result)
        {
            _environmentService.JoinableTaskFactory.RunAsync(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                var runDocument = _mutations.FirstOrDefault(r => r.Document.Id == result.Id);

                if (runDocument != null)
                {
                    runDocument.Result = result;

                    if (result.CompilationResult != null && !result.CompilationResult.IsSuccess)
                    {
                        runDocument.Status = MutationRunItem.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = "Failed to compile.";
                        return;
                    }

                    if (result.UnexpectedError != null)
                    {
                        runDocument.Status = MutationRunItem.TestRunStatusEnum.CompletedWithUnknownReason;
                        runDocument.InfoText = result.UnexpectedError;
                        return;
                    }

                    runDocument.Status = result.Survived
                        ? MutationRunItem.TestRunStatusEnum.CompleteAndSurvived
                        : MutationRunItem.TestRunStatusEnum.CompleteAndKilled;

                    runDocument.InfoText = $"{result.FailedTests.Count} of {result.TestsRunCount} tests failed";
                    runDocument.InfoText += result.Survived ? " (mutation survived)" : " (mutation killed)";
                }
            });
        }

        public bool AnyMutationThatMatchFilePath(Document document)
        {
            if (_mutations == null || !_mutations.Any())
            {
                return false;
            }

            return _environmentService.JoinableTaskFactory.Run(async () =>
            {
                await _environmentService.JoinableTaskFactory.SwitchToMainThreadAsync();

                #pragma warning disable VSTHRD010
                return _mutations.Any(m => m.Document.FilePath == document.FullName);
            });
        }

        public IEnumerable<MutationRunItem> GetSurvivedMutations()
        {
            return _mutations.Where(m => m.Status == MutationRunItem.TestRunStatusEnum.CompleteAndSurvived).ToList();
        }
    }
}
