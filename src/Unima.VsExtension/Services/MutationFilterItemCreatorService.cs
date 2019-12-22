using System;
using System.Collections.Generic;
using Unima.Core.Creator.Filter;

namespace Unima.VsExtension.Services
{
    public class MutationFilterItemCreatorService
    {
        public IEnumerable<MutationDocumentFilterItem> CreateFilterFromFilePaths(IEnumerable<string> files)
        {
            var mutationFilterItems = new List<MutationDocumentFilterItem>();

            foreach (var file in files)
            {
                mutationFilterItems.Add(new MutationDocumentFilterItem
                {
                    Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                    Resource = file
                });
            }

            return mutationFilterItems;
        }

        public IEnumerable<MutationDocumentFilterItem> CreateFilterFromLines(string file, int startLine, int endLine)
        {
            var actualStartLine = startLine > endLine ? endLine : startLine;

            var mutationFilterItems = new List<MutationDocumentFilterItem>
            {
                new MutationDocumentFilterItem
                {
                    Effect = MutationDocumentFilterItem.FilterEffect.Allow,
                    Resource = file,
                    Lines = new List<string> { $"{actualStartLine},{Math.Abs(startLine - endLine)}" }
                }
            };

            return mutationFilterItems;
        }
    }
}
