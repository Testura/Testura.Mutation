using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph
{
    [Export(typeof(IGlyphFactoryProvider))]
    [Name("MutationCodeHighlightGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(MutationCodeHighlightTag))]
    public class MutationCodeHighlightGlyphFactoryProvider : IGlyphFactoryProvider
    {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            return new MutationCodeHighlightGlyphFactory();
        }
    }
}
