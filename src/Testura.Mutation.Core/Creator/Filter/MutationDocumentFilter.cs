using System.Collections.Generic;
using System.Linq;

namespace Testura.Mutation.Core.Creator.Filter
{
    public class MutationDocumentFilter
    {
        public List<MutationDocumentFilterItem> FilterItems { get; set; }

        public bool ResourceAllowed(string resource)
        {
            if (FilterItems == null || !FilterItems.Any())
            {
                return true;
            }

            if (FilterItems.Any(m => m.IsDenied(resource)))
            {
                return false;
            }

            if (FilterItems.Any(m => m.IsAllowed(resource)))
            {
                return true;
            }

            return false;
        }

        public bool ResourceLinesAllowed(string resource, int line)
        {
            if (FilterItems == null)
            {
                return true;
            }

            var matchingFilterItems = FilterItems.Where(m => m.MatchResource(resource));

            if (matchingFilterItems.All(m => m.Lines == null || !m.Lines.Any()))
            {
                return true;
            }

            if (matchingFilterItems.Any(m => m.LineAreDenied(line)))
            {
                return false;
            }

            if (!matchingFilterItems.Any(m => m.Effect == MutationDocumentFilterItem.FilterEffect.Allow))
            {
                return true;
            }

            if (matchingFilterItems.Any(m => m.LineAreAllowed(line)))
            {
                return true;
            }

            return false;
        }
    }
}
