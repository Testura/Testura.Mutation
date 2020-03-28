namespace Testura.Mutation.Core
{
    public class MutationCategory
    {
        public MutationCategory(string headCategory, string subcategory)
        {
            HeadCategory = headCategory;
            Subcategory = subcategory;
        }

        public MutationCategory()
        {
        }

        public string HeadCategory { get; set; }

        public string Subcategory { get; set; }
    }
}
