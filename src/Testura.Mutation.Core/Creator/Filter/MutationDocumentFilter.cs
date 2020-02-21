using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Creator.Filter
{
    public class MutationDocumentFilter
    {
        public MutationDocumentFilter()
        {
            FilterItems = new List<MutationDocumentFilterItem>();
        }

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

        public bool ResourceLinesAllowed(string resource, int line, SyntaxNode code)
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

            if (matchingFilterItems.All(m => m.Effect != MutationDocumentFilterItem.FilterEffect.Allow))
            {
                return true;
            }

            if (matchingFilterItems.Any(m => m.LineAreAllowed(line)))
            {
                return true;
            }

            return false;
        }

        public bool CodeAllowed(string resource, int line, SyntaxNode code)
        {
            if (FilterItems == null)
            {
                return true;
            }

            var matchingFilterItems = FilterItems.Where(m => m.MatchResource(resource));

            if (matchingFilterItems.All(m => string.IsNullOrEmpty(m.CodeConstrain)))
            {
                return true;
            }

            if (matchingFilterItems.Any(m => m.LineAreAllowedWithCodeConstrain(line, code)))
            {
                return true;
            }

            if (matchingFilterItems.Any(m => m.LinesAreDeniedWithCodeConstrain(line, code)))
            {
                return false;
            }

            return true;
        }
    }
}
