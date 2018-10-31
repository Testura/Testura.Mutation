using System;
using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Creator.Filter
{
    public class MutationDocumentFilterCreator
    {
        private const string LineFlag = ":@:";

        public IList<MutationDocumentFilterItem> CreateFilterItems(IList<string> filterItems)
        {
            if (filterItems == null || !filterItems.Any())
            {
                return new List<MutationDocumentFilterItem> { new MutationDocumentEmptyFilterItem() };
            }

            return CreateFilterWithLines(filterItems);
        }

        private IList<MutationDocumentFilterItem> CreateFilterWithLines(IEnumerable<string> filterItems)
        {
            var mutationDocumentFilterItems = new List<MutationDocumentFilterItem>();
            foreach (var filterItem in filterItems)
            {
                if (!filterItem.Contains(LineFlag))
                {
                    mutationDocumentFilterItems.Add(new MutationDocumentFilterItem { Name = filterItem });
                    continue;
                }

                var nameAndLinesSplit = filterItem.Split(new[] { LineFlag }, StringSplitOptions.RemoveEmptyEntries);
                var lineSplit = nameAndLinesSplit[1].Split(':');
                var lines = new List<int>();

                foreach (var line in lineSplit)
                {
                    if (line.Contains(","))
                    {
                        var singleLineSplit = line.Split(',');
                        var start = int.Parse(singleLineSplit[0]);
                        lines.Add(start);

                        for (int n = 0; n < int.Parse(singleLineSplit[1]); n++)
                        {
                            lines.Add(start + n + 1);
                        }

                        continue;
                    }

                    lines.Add(int.Parse(line));
                }

                if (mutationDocumentFilterItems.Any(m => m.Name == nameAndLinesSplit[0]))
                {
                    mutationDocumentFilterItems.FirstOrDefault(m => m.Name == nameAndLinesSplit[0]).LineNumbers.AddRange(lines);
                }
                else
                {
                    mutationDocumentFilterItems.Add(new MutationDocumentFilterItem() { Name = nameAndLinesSplit[0], LineNumbers = lines });
                }
            }

            return mutationDocumentFilterItems;
        }
    }
}
