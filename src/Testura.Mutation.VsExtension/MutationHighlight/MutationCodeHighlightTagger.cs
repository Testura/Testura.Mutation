using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTagger : ITagger<MutationCodeHighlightTag>
    {
        private IList<MutationHightlight> _mutations;

        public MutationCodeHighlightTagger(
            ITextView view,
            ITextBuffer sourceBuffer,
            ITextSearchService textSearchService,
            ITextStructureNavigator textStructureNavigator)
        {
            View = view;
            SourceBuffer = sourceBuffer;
            TextSearchService = textSearchService;
            TextStructureNavigator = textStructureNavigator;
            _mutations = new List<MutationHightlight>();
            MutationCodeHighlightHandler.OnMutationHighlightUpdate += MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ITextView View { get; set; }

        public ITextBuffer SourceBuffer { get; set; }

        public ITextSearchService TextSearchService { get; set; }

        public ITextStructureNavigator TextStructureNavigator { get; set; }

        public IEnumerable<ITagSpan<MutationCodeHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            SourceBuffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);

            var mutations = _mutations.Where(m => m.FilePath == document.FilePath);

            var res =
                new List<ITagSpan<MutationCodeHighlightTag>>();

            foreach (var mutationHightlight in mutations)
            {
                var span = new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(mutationHightlight.Start, mutationHightlight.Length));
                res.Add(new TagSpan<MutationCodeHighlightTag>(span, new MutationCodeHighlightTag()));
            }

            return res;
        }

        private void MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(object sender, IList<MutationHightlight> e)
        {
            if (_mutations.Count == 0 && e.Count == 0)
            {
                return;
            }

            _mutations = e;
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(0, SourceBuffer.CurrentSnapshot.Length - 1))));
        }
    }
}
