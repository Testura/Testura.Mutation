using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Testura.Mutation.VsExtension.Models;
using Testura.Mutation.VsExtension.MutationHighlight.Definitions;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightTagger : ITagger<MutationCodeHighlightTag>, IDisposable
    {
        private readonly VisualStudioWorkspace _visualStudioWorkspace;
        private readonly Dictionary<MutationRunItem.TestRunStatusEnum, string> _mutationDefinitions;
        private readonly DocumentId _documentId;
        private IList<MutationHighlight> _mutations;
        private Dictionary<ITrackingSpan, string> _trackingSpans;

        public MutationCodeHighlightTagger(
            VisualStudioWorkspace visualStudioWorkspace,
            ITextView view,
            ITextBuffer sourceBuffer,
            ITextSearchService textSearchService,
            ITextStructureNavigator textStructureNavigator)
        {
            _visualStudioWorkspace = visualStudioWorkspace;
            _trackingSpans = new Dictionary<ITrackingSpan, string>();

            View = view;
            SourceBuffer = sourceBuffer;
            TextSearchService = textSearchService;
            TextStructureNavigator = textStructureNavigator;

            _mutationDefinitions = new Dictionary<MutationRunItem.TestRunStatusEnum, string>
            {
                [MutationRunItem.TestRunStatusEnum.Waiting] = nameof(MutationNotRunFormatDefinition),
                [MutationRunItem.TestRunStatusEnum.Running] = nameof(MutationNotRunFormatDefinition),
                [MutationRunItem.TestRunStatusEnum.CompleteAndKilled] = nameof(MutationKilledFormatDefinition),
                [MutationRunItem.TestRunStatusEnum.CompleteAndSurvived] = nameof(MutationSurvivedFormatDefinition),
                [MutationRunItem.TestRunStatusEnum.CompletedWithUnknownReason] = nameof(MutationUnknownErrorFormatDefinition)
            };

            MutationCodeHighlightHandler.OnMutationHighlightUpdate += MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
            visualStudioWorkspace.WorkspaceChanged += VisualStudioWorkspaceOnWorkspaceChanged;

            SourceBuffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);

            var documentIds = visualStudioWorkspace.CurrentSolution.GetDocumentIdsWithFilePath(document.FilePath);
            _documentId = documentIds.Any() ? documentIds.First() : null;

            MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(this, MutationCodeHighlightHandler.MutationHighlights);
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
                        var mutation = _mutations.FirstOrDefault(m => m.Mutation.Document.Id == _trackingSpans[trackingSpan]);

                        if (mutation == null)
                        {
                            continue;
                        }

                        tags.Add(new TagSpan<MutationCodeHighlightTag>(snapshotSpan, new MutationCodeHighlightTag(_mutationDefinitions[mutation.Mutation.Status], mutation)));
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
            _visualStudioWorkspace.WorkspaceChanged -= VisualStudioWorkspaceOnWorkspaceChanged;
        }

        private void CreateTrackingSpans()
        {
            _trackingSpans = new Dictionary<ITrackingSpan, string>();

            if (_mutations == null || !_mutations.Any())
            {
                return;
            }

            var currentSnapshot = SourceBuffer.CurrentSnapshot;

            SourceBuffer.Properties.TryGetProperty(
                typeof(ITextDocument), out ITextDocument document);

            foreach (var mutationHightlight in _mutations.Where(m => m.Mutation.Document.FilePath == document?.FilePath))
            {
                var mutationSpan = mutationHightlight.Mutation.Document.MutationDetails.Orginal.FullSpan;

                var span = new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(mutationSpan.Start, mutationSpan.Length));
                var trackingSpan = currentSnapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeExclusive);
                _trackingSpans.Add(trackingSpan, mutationHightlight.Mutation.Document.Id);
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

        private void MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(object sender, IList<MutationHighlight> e)
        {
            if (_mutations != null && _mutations.Count == 0 && e.Count == 0)
            {
                return;
            }

            if (SourceBuffer?.CurrentSnapshot == null || SourceBuffer.CurrentSnapshot.Length == 0)
            {
                return;
            }

            _mutations = new List<MutationHighlight>(e ?? new List<MutationHighlight>());

            CreateTrackingSpans();
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, new Span(0, SourceBuffer.CurrentSnapshot.Length - 1))));
        }
    }
}
