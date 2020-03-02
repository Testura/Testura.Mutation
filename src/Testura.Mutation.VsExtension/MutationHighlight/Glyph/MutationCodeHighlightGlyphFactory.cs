using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph
{
    public class MutationCodeHighlightGlyphFactory : IGlyphFactory
    {
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            return new MutationCodeHighlightGlyph();
        }
    }
}
