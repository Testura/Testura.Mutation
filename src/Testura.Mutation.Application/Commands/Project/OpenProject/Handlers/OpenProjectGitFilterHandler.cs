using System.Collections.Generic;
using System.IO;
using System.Threading;
using log4net;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectGitFilterHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectGitFilterHandler));

        private readonly MutationDocumentFilterItemGitDiffCreator _diffCreator;

        public OpenProjectGitFilterHandler(MutationDocumentFilterItemGitDiffCreator diffCreator)
        {
            _diffCreator = diffCreator;
        }

        public IList<MutationDocumentFilterItem> CreateGitFilterItems(string solutionPath, GitInfo gitInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var filterItems = new List<MutationDocumentFilterItem>();

            if (gitInfo != null && gitInfo.GenerateFilterFromDiffWithMaster)
            {
                Log.Info("Creating filter items from git diff with master");
                filterItems.AddRange(_diffCreator.GetFilterItemsFromDiff(Path.GetDirectoryName(solutionPath)));
            }

            return filterItems;
        }
    }
}
