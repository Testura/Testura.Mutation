using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Unima.VsExtension.MutationHighlight
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [TagType(typeof(TextMarkerTag))]
    public class MutationCodeHighlightTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal ITextSearchService TextSearchService { get; set; }

        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer)
            where T : ITag
        {
            if (textView.TextBuffer != buffer)
            {
                return null;
            }

            ITextStructureNavigator textStructureNavigator =
                TextStructureNavigatorSelector.GetTextStructureNavigator(buffer);

            return new MutationCodeHighlightTagger(textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
        }
    }
}
