using System.ComponentModel.Composition;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(MutationCodeHighlightTag))]
    public class MutationCodeHighlightTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal VisualStudioWorkspace Workspace { get; set; }

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

            return new MutationCodeHighlightTagger(Workspace, textView, buffer, TextSearchService, textStructureNavigator) as ITagger<T>;
        }
    }
}
