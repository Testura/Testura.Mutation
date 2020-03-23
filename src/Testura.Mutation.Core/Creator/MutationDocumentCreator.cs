using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;

namespace Testura.Mutation.Core.Creator
{
    public class MutationDocumentCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MutationDocumentCreator));

        public IList<MutationDocument> CreateMutations(MutationConfig config, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Log.Info("Starting to analyze test..");

                var mutations = new List<MutationDocument>();

                foreach (var mutationProjectInfo in config.MutationProjects)
                {
                    var currentProject = config.Solution.Projects.FirstOrDefault(p => p.Name == mutationProjectInfo.Project.Name);

                    if (currentProject == null)
                    {
                        Log.Error($"Could not find any project with the name {mutationProjectInfo.Project.Name}");
                        continue;
                    }

                    var documentIds = currentProject.DocumentIds;

                    Log.Info($"Starting to create mutations for {currentProject.Name}..");
                    foreach (var documentId in documentIds)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        mutations.AddRange(CreateMutationsForDocument(config, currentProject, documentId));
                    }
                }

                if (!mutations.Any())
                {
                    Log.Warn("Could not find a single mutation. Maybe check your filter?");
                }

                return mutations;
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancellation requested");
                return new List<MutationDocument>();
            }
            catch (Exception ex)
            {
                Log.Error("Unknown exception when creating mutation documents", ex);
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
                    Log.Info($"Ignoring {document.Name}.");
                    return mutations;
                }

                Log.Info($"Creating mutation for {document.Name}..");

                var root = document.GetSyntaxRootAsync().Result;

                foreach (var mutationOperator in config.Mutators)
                {
                    var mutatedDocuments = mutationOperator.GetMutatedDocument(root, document);
                    mutations.AddRange(mutatedDocuments.Where(m =>
                        config.Filter == null ||
                        (config.Filter.ResourceLinesAllowed(document.FilePath, GetDocumentLine(m))
                         && config.Filter.CodeAllowed(document.FilePath, GetDocumentLine(m), m.MutationDetails.Original))));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error when creating mutation: " + ex.Message);
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
