using System.Collections.Generic;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Core.Creator.Filter
{
    public class MutationDocumentFilterItemGitDiffCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MutationDocumentFilterItemGitDiffCreator));

        private readonly IGitDiff _gitDiff;

        public MutationDocumentFilterItemGitDiffCreator(IGitDiff gitDiff)
        {
            _gitDiff = gitDiff;
        }

        public IList<MutationDocumentFilterItem> GetFilterItemsFromDiff(string path, string branch)
        {
            var diff = _gitDiff.GetDiff(path, branch);

            var matches = Regex.Matches(diff.Replace(System.Environment.NewLine, string.Empty), @"^\+\+\+ .\/(.*)$|^@@.+\+(.*) @@", RegexOptions.Multiline);
            var filterItems = new List<MutationDocumentFilterItem>();

            Log.Info("Filter item(s) created: ");

            for (int n = 0; n < matches.Count; n++)
            {
                var fileName = matches[n].Groups[1].Value;
                var lines = new List<string>();

                for (int i = n + 1; i < matches.Count; i++)
                {
                    if (matches[i].Value.Contains("+++"))
                    {
                        n -= 1;
                        break;
                    }

                    lines.Add(matches[i].Groups[2].Value);
                    n = i + 1;
                }

                filterItems.Add(new MutationDocumentFilterItem
                {
                    Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                    Resource = $"*{fileName}",
                    Lines = lines
                });

                Log.Info(JsonConvert.SerializeObject(filterItems[filterItems.Count - 1]));
            }

            Log.Info($"Final count: {filterItems.Count}");

            return filterItems;
        }
    }
}
