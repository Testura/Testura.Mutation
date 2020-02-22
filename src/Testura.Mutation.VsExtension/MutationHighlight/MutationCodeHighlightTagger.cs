using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Testura.Mutation.VsExtension.MutationHighlight.Definitions;
using TestRunDocument = Testura.Mutation.VsExtension.Models.TestRunDocument;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTagger : ITagger<MutationCodeHighlightTag>, IDisposable
    {
        private readonly Dictionary<TestRunDocument.TestRunStatusEnum, string> _mutationDefinitions;
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

            _mutations = MutationCodeHighlightHandler.MutationHighlights ?? new List<MutationHightlight>();
            _mutationDefinitions = new Dictionary<TestRunDocument.TestRunStatusEnum, string>
            {
                [TestRunDocument.TestRunStatusEnum.Waiting] = nameof(MutationNotRunFormatDefinition),
                [TestRunDocument.TestRunStatusEnum.Running] = nameof(MutationNotRunFormatDefinition),
                [TestRunDocument.TestRunStatusEnum.CompleteAndKilled] = nameof(MutationKilledFormatDefinition),
                [TestRunDocument.TestRunStatusEnum.CompleteAndSurvived] = nameof(MutationSurvivedFormatDefinition),
                [TestRunDocument.TestRunStatusEnum.CompletedWithUnknownReason] = nameof(MutationUnknownErrorFormatDefinition)
            };

            MutationCodeHighlightHandler.OnMutationHighlightUpdate += MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ITextView View { get; set; }

        public ITextBuffer SourceBuffer { get; set; }

        public ITextSearchService TextSearchService { get; set; }

        public ITextStructureNavigator TextStructureNavigator { get; set; }

        public IEnumerable<ITagSpan<MutationCodeHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            try
            {
                SourceBuffer.Properties.TryGetProperty(
                    typeof(ITextDocument), out ITextDocument document);

                var mutations = _mutations.Where(m => m.FilePath == document.FilePath);

                var res = new List<ITagSpan<MutationCodeHighlightTag>>();

                foreach (var mutationHightlight in mutations)
                {
                    var span = new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(mutationHightlight.Start, mutationHightlight.Length));
                    res.Add(new TagSpan<MutationCodeHighlightTag>(span, new MutationCodeHighlightTag(_mutationDefinitions[mutationHightlight.Status])));
                }

                return res;
            }
            catch (Exception)
            {
                return new List<ITagSpan<MutationCodeHighlightTag>>();
            }
        }

        public void Dispose()
        {
            MutationCodeHighlightHandler.OnMutationHighlightUpdate -= MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        private void MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(object sender, IList<MutationHightlight> e)
        {
            if (_mutations.Count == 0 && e.Count == 0)
            {
                return;
            }

            if (SourceBuffer?.CurrentSnapshot == null || SourceBuffer.CurrentSnapshot.Length == 0)
            {
                return;
            }

            _mutations = new List<MutationHightlight>(e);

            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(0, SourceBuffer.CurrentSnapshot.Length - 1))));
        }
    }
}
