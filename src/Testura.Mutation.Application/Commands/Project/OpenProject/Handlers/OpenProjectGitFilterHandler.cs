using System.Collections.Generic;
using System.IO;
using System.Threading;
using log4net;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
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

        public void InitializeGitFilter(string solutionPath, GitInfo gitInfo, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (gitInfo != null && gitInfo.GenerateFilterFromDiffWithMaster)
            {
                Log.Info("Creating filter items from git diff with master");

                var filterItems = _diffCreator.GetFilterItemsFromDiff(Path.GetDirectoryName(solutionPath), string.Empty);

                if (applicationConfig.Filter == null)
                {
                    applicationConfig.Filter = new MutationDocumentFilter { FilterItems = new List<MutationDocumentFilterItem>() };
                }

                applicationConfig.Filter.FilterItems.AddRange(filterItems);
            }
        }
    }
}
