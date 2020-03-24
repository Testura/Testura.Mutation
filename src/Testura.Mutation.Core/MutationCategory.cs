namespace Testura.Mutation.Core
{
    public class MutationCategory
    {
        public MutationCategory(MutationOperators headCategory, string subcategory)
        {
            HeadCategory = headCategory;
            Subcategory = subcategory;
        }

        public MutationCategory()
        {
        }

        public MutationOperators HeadCategory { get; set; }

        public string Subcategory { get; set; }
    }
}
