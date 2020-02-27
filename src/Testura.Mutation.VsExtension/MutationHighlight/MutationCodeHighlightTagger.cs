using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
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
        private readonly VisualStudioWorkspace _visualStudioWorkspace;
        private readonly Dictionary<TestRunDocument.TestRunStatusEnum, string> _mutationDefinitions;
        private readonly DocumentId _documentId;
        private IList<MutationHightlight> _mutations;
        private Dictionary<ITrackingSpan, string> _trackingSpans;

        public MutationCodeHighlightTagger(
            VisualStudioWorkspace visualStudioWorkspace,
            ITextView view,
            ITextBuffer sourceBuffer,
            ITextSearchService textSearchService,
            ITextStructureNavigator textStructureNavigator)
        {
            _visualStudioWorkspace = visualStudioWorkspace;
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
            visualStudioWorkspace.WorkspaceChanged += VisualStudioWorkspaceOnWorkspaceChanged;

            SourceBuffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);

            var documentIds = visualStudioWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath);
            _documentId = documentIds.Any() ? documentIds.First() : null;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public ITextView View { get; set; }

        public ITextBuffer SourceBuffer { get; set; }

        public ITextSearchService TextSearchService { get; set; }

        public ITextStructureNavigator TextStructureNavigator { get; set; }

        public void VisualStudioWorkspaceOnWorkspaceChanged(object sender, WorkspaceChangeEventArgs e)
        {
            if (e.Kind == WorkspaceChangeKind.DocumentChanged && e.DocumentId == _documentId)
            {
                RemoveEmptyTrackingSpans();
            }
        }

        public IEnumerable<ITagSpan<MutationCodeHighlightTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            try
            {
                var tags = new List<ITagSpan<MutationCodeHighlightTag>>();

                var currentSnapshot = SourceBuffer.CurrentSnapshot;
                foreach (var trackingSpan in _trackingSpans.Keys)
                {
                    var spanInCurrentSnapshot = trackingSpan.GetSpan(currentSnapshot);
                    if (spans.Any(sp => spanInCurrentSnapshot.IntersectsWith(sp)))
                    {
                        var snapshotSpan = new SnapshotSpan(currentSnapshot, spanInCurrentSnapshot);
                        tags.Add(new TagSpan<MutationCodeHighlightTag>(snapshotSpan, new MutationCodeHighlightTag(_mutationDefinitions[(TestRunDocument.TestRunStatusEnum)int.Parse(_trackingSpans[trackingSpan])])));
                    }
                }

                return tags;
            }
            catch (Exception)
            {
                return new List<ITagSpan<MutationCodeHighlightTag>>();
            }
        }

        public void Dispose()
        {
            MutationCodeHighlightHandler.OnMutationHighlightUpdate -= MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
            _visualStudioWorkspace.WorkspaceChanged += VisualStudioWorkspaceOnWorkspaceChanged;
        }

        private void CreateTrackingSpans()
        {
            _trackingSpans = new Dictionary<ITrackingSpan, string>();
            var currentSnapshot = SourceBuffer.CurrentSnapshot;

            SourceBuffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);

            foreach (var mutationHightlight in _mutations.Where(m => m.FilePath == document.FilePath))
            {
                var span = new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(mutationHightlight.Start, mutationHightlight.Length));
                var trackingSpan = currentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                _trackingSpans.Add(trackingSpan, ((int)mutationHightlight.Status).ToString());
            }
        }

        private void RemoveEmptyTrackingSpans()
        {
            var currentSnapshot = SourceBuffer.CurrentSnapshot;
            var keysToRemove = _trackingSpans.Keys.Where(ts => ts.GetSpan(currentSnapshot).Length == 0).ToList();
            foreach (var key in keysToRemove)
            {
                _trackingSpans.Remove(key);
            }
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

            CreateTrackingSpans();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(0, SourceBuffer.CurrentSnapshot.Length - 1))));
        }
    }
}
