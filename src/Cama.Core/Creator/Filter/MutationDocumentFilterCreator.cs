using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Creator.Filter
{
    public class MutationDocumentFilterCreator
    {
        private const string GitLineFlag = "@@ ";

        public IList<MutationDocumentFilterItem> CreateFilterItems(IEnumerable<string> filterItems)
        {
            if (filterItems.Any(f => f.StartsWith("@@ ")))
            {
                return CreateFilterWithLines(filterItems);
            }

            return filterItems.Select(f => new MutationDocumentFilterItem { Name = f }).ToList();
        }

        private IList<MutationDocumentFilterItem> CreateFilterWithLines(IEnumerable<string> filterItems)
        {
            var mutationDocumentFilterItems = new List<MutationDocumentFilterItem>();
            foreach (var filterItem in filterItems)
            {
                
            }
        }
    }
}
