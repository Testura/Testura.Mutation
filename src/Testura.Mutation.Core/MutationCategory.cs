namespace Testura.Mutation.Core
{
    public class MutationCategory
    {
        public MutationCategory(MutationOperators category, string subcategory)
        {
            Category = category;
            Subcategory = subcategory;
        }

        public MutationCategory()
        {
        }

        public MutationOperators Category { get; set; }

        public string Subcategory { get; set; }
    }
}
