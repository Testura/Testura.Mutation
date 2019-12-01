using System.Collections.Generic;
using System.Text.RegularExpressions;
using Anotar.Log4Net;
using Newtonsoft.Json;
using Unima.Core.Git;

namespace Unima.Core.Creator.Filter
{
    public class MutationDocumentFilterItemGitDiffCreator
    {
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

            LogTo.Info("Filter item(s) created: ");

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

                LogTo.Info(JsonConvert.SerializeObject(filterItems[filterItems.Count - 1]));
            }

            LogTo.Info($"Final count: {filterItems.Count}");

            return filterItems;
        }
    }
}
