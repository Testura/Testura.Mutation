using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Creator.Filter;
using Cama.Core.Creator.Mutators;
using Cama.Core.Exceptions;
using Microsoft.CodeAnalysis.MSBuild;

namespace Cama.Core.Creator
{
    public class MutationDocumentCreator
    {
        private MutationDocumentFilterCreator _mutationDocumentFilterCreator;

        public MutationDocumentCreator()
        {
            _mutationDocumentFilterCreator = new MutationDocumentFilterCreator();
        }

        public async Task<IList<MutationDocument>> CreateMutationsAsync(CamaConfig config, IList<IMutator> mutationOperators)
        {
            try
            {
                var workspace = MSBuildWorkspace.Create();

                LogTo.Info("Opening solution..");
                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);
                LogTo.Info("Starting to analyze test..");

                var list = new List<MutationDocument>();
                var filters = _mutationDocumentFilterCreator.CreateFilterItems(config.Filter);

                foreach (var mutationProjectInfo in config.MutationProjects)
                {
                    var currentProject = solution.Projects.FirstOrDefault(p => p.Name == mutationProjectInfo.Name);

                    if (currentProject == null)
                    {
                        LogTo.Error($"Could not find any project with the name {mutationProjectInfo.Name}");
                        continue;
                    }

                    var documentIds = currentProject.DocumentIds;

                    LogTo.Info("Starting to create mutations..");
                    foreach (var documentId in documentIds)
                    {
                        try
                        {
                            var document = currentProject.GetDocument(documentId);
                            var filter = filters.FirstOrDefault(f => f.MatchFilterName(document.Name));

                            if (filter == null)
                            {
                                LogTo.Info($"Ignoring {document.Name} (not in filter).");
                                continue;
                            }

                            LogTo.Info($"Creating mutation for {document.Name}..");

                            var root = document.GetSyntaxRootAsync().Result;
                            var mutationDocuments = new List<MutationDocument>();

                            foreach (var mutationOperator in mutationOperators)
                            {
                                var mutatedDocuments = mutationOperator.GetMutatedDocument(root, document);
                                mutationDocuments.AddRange(mutatedDocuments.Where(m => filter.MatchFilterLines(m)));
                            }

                            list.AddRange(mutationDocuments);
                        }
                        catch (Exception ex)
                        {
                            LogTo.Error("Error when creating mutation: " + ex.Message);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Unknown exception when creating mutation documents", ex);
                throw new MutationDocumentException("Unknown exception when creating mutation documents", ex);
            }
        }
    }
}
