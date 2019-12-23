using Microsoft.VisualStudio.Text.Tagging;

namespace Unima.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTag : TextMarkerTag
    {
        public MutationCodeHighlightTag()
            : base("MarkerFormatDefinition/MutationCodeHighlightFormatDefinition")
        {
        }
    }
}
