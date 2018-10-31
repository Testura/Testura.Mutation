using System.Collections.Generic;

namespace Cama.Core.Creator.Filter
{
    public class MutationDocumentFilterItem
    {
        public MutationDocumentFilterItem()
        {
            Lines = new List<int>();
        }

        public string Name { get; set; }

        public IList<int> Lines { get; set; }
    }
}
