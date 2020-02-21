using TestRunDocument = Testura.Mutation.VsExtension.Models.TestRunDocument;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationHightlight
    {
        public string FilePath { get; set; }

        public string MutationText { get; set; }

        public int Line { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public TestRunDocument.TestRunStatusEnum Status { get; set; }
    }
}
