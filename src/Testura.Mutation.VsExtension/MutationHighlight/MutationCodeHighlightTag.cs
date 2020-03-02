using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTag : TextMarkerTag, IGlyphTag
    {
        public MutationCodeHighlightTag(string type)
            : base($"MarkerFormatDefinition/{type}")
        {
        }
    }
}
