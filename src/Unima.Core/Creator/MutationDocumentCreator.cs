using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Unima.Core.Config;
using Unima.Core.Exceptions;
using Unima.Core.Solution;

namespace Unima.Core.Creator
{
    public class MutationDocumentCreator
    {
        private readonly ISolutionOpener _solutionOpener;

        public MutationDocumentCreator(ISolutionOpener solutionOpener)
        {
            _solutionOpener = solutionOpener;
        }

        public async Task<IList<MutationDocument>> CreateMutationsAsync(UnimaConfig config)
        {
            try
            {
                LogTo.Info("Opening solution..");
                var solution = await _solutionOpener.GetSolutionAsync(config);
                LogTo.Info("Starting to analyze test..");

                var list = new List<MutationDocument>();

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
                        try
                        {
                            var document = currentProject.GetDocument(documentId);

                            if (config.Filter != null && !config.Filter.ResourceAllowed(document.FilePath))
                            {
                                LogTo.Info($"Ignoring {document.Name}.");
                                continue;
                            }

                            LogTo.Info($"Creating mutation for {document.Name}..");

                            var root = document.GetSyntaxRootAsync().Result;
                            var mutationDocuments = new List<MutationDocument>();

                            foreach (var mutationOperator in config.Mutators)
                            {
                                var mutatedDocuments = mutationOperator.GetMutatedDocument(root, document);
                                mutationDocuments.AddRange(mutatedDocuments.Where(m =>
                                    config.Filter == null ||
                                    config.Filter.ResourceLinesAllowed(document.FilePath, GetDocumentLine(m))));
                            }

                            list.AddRange(mutationDocuments);
                        }
                        catch (Exception ex)
                        {
                            LogTo.Error("Error when creating mutation: " + ex.Message);
                        }
                    }
                }

                if (!list.Any())
                {
                    LogTo.Warn("Could not find a single mutation. Maybe check your filter?");
                }

                return list;
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Unknown exception when creating mutation documents", ex);
                throw new MutationDocumentException("Unknown exception when creating mutation documents", ex);
            }
        }

        private int GetDocumentLine(MutationDocument mutationDocument)
        {
            var line = mutationDocument.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries).First();
            return int.Parse(line);
        }
    }
}
