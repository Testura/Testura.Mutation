using Microsoft.VisualStudio.Text.Tagging;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTag : TextMarkerTag
    {
        public MutationCodeHighlightTag()
            : base("MarkerFormatDefinition/MutationCodeHighlightFormatDefinition")
        {
        }
    }
}
