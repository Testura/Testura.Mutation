using System;
using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Creator.Filter
{
    public class MutationDocumentFilterItem
    {
        public MutationDocumentFilterItem()
        {
            LineNumbers = new List<int>();
        }

        public string Name { get; set; }

        public List<int> LineNumbers { get; set; }

        public virtual bool MatchFilterName(string documentName)
        {
            return Name.EndsWith(documentName);
        }

        public virtual bool MatchFilterLines(MutationDocument mutationDocument)
        {
            if (!LineNumbers.Any())
            {
                return true;
            }

            var line = mutationDocument.MutationDetails.Location.Line.Split(new[] { "@(", ":" }, StringSplitOptions.RemoveEmptyEntries).First();
            return LineNumbers.Contains(int.Parse(line));
        }
    }
}
