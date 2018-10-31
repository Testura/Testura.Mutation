namespace Cama.Core.Creator.Filter
{
    public class MutationDocumentEmptyFilterItem : MutationDocumentFilterItem
    {
        public override bool MatchFilterName(string documentName)
        {
            return true;
        }

        public override bool MatchFilterLines(MutationDocument mutationDocument)
        {
            return true;
        }
    }
}
