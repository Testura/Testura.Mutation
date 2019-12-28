using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Core.Creator
{
    public class MutationDocumentCreator
    {
        private readonly ISolutionOpener _solutionOpener;

        public MutationDocumentCreator(ISolutionOpener solutionOpener)
        {
            _solutionOpener = solutionOpener;
        }

        public async Task<IList<MutationDocument>> CreateMutationsAsync(MutationConfig config, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                LogTo.Info("Opening solution..");
                var solution = await _solutionOpener.GetSolutionAsync(config);
                LogTo.Info("Starting to analyze test..");

                var mutations = new List<MutationDocument>();

                foreach (var mutationProjectInfo in config.MutationProjects)
                {
                    var currentProject = solution.Projects.FirstOrDefault(p => p.Name == mutationProjectInfo.Name);

                    if (currentProject == null)
                    {
                        LogTo.Error($"Could not find any project with the name {mutationProjectInfo.Name}");
                        continue;
                    }

                    var documentIds = currentProject.DocumentIds;

                    LogTo.Info($"Starting to create mutations for {currentProject.Name}..");
                    foreach (var documentId in documentIds)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        mutations.AddRange(CreateMutationsForDocument(config, currentProject, documentId));
                    }
                }

                if (!mutations.Any())
                {
                    LogTo.Warn("Could not find a single mutation. Maybe check your filter?");
                }

                return mutations;
            }
            catch (OperationCanceledException)
            {
                LogTo.Info("Cancellation requested");
                return new List<MutationDocument>();
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Unknown exception when creating mutation documents", ex);
                throw new MutationDocumentException("Unknown exception when creating mutation documents", ex);
            }
        }

        private IList<MutationDocument> CreateMutationsForDocument(MutationConfig config, Project currentProject, DocumentId documentId)
        {
            var mutations = new List<MutationDocument>();
            try
            {
                var document = currentProject.GetDocument(documentId);

                if (config.Filter != null && !config.Filter.ResourceAllowed(document.FilePath))
                {
                    LogTo.Info($"Ignoring {document.Name}.");
                    return mutations;
                }

                LogTo.Info($"Creating mutation for {document.Name}..");

                var root = document.GetSyntaxRootAsync().Result;

                foreach (var mutationOperator in config.Mutators)
                {
                    var mutatedDocuments = mutationOperator.GetMutatedDocument(root, document);
                    mutations.AddRange(mutatedDocuments.Where(m =>
                        config.Filter == null ||
                        config.Filter.ResourceLinesAllowed(document.FilePath, GetDocumentLine(m))));
                }
            }
            catch (Exception ex)
            {
                LogTo.Error("Error when creating mutation: " + ex.Message);
            }

            return mutations;
        }

        private int GetDocumentLine(MutationDocument mutationDocument)
        {
            var line = mutationDocument.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries).First();
            return int.Parse(line);
        }
    }
}
