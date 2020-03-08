using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace Testura.Mutation.VsExtension.MutationHighlight.QuickInfo
{
    public class MutationCodeHighlightQuckInfo : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private IList<MutationHighlight> _mutations;

        public MutationCodeHighlightQuckInfo(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
            _mutations = MutationCodeHighlightHandler.MutationHighlights ?? new List<MutationHighlight>();

            MutationCodeHighlightHandler.OnMutationHighlightUpdate += MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);

            if (triggerPoint == null || !_mutations.Any())
            {
                return Task.FromResult<QuickInfoItem>(null);
            }

            var line = triggerPoint.Value.GetContainingLine();
            var mutation = _mutations.FirstOrDefault(m =>
                InsideSpan(m.Mutation.Document.MutationDetails.Orginal.FullSpan.Start, line) ||
                InsideSpan(m.Mutation.Document.MutationDetails.Orginal.FullSpan.Start + m.Mutation.Document.MutationDetails.Orginal.FullSpan.Length, line));

            if (mutation == null)
            {
                return Task.FromResult<QuickInfoItem>(null);
            }

            var lineSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive);

            var text = triggerPoint.Value.GetContainingLine().GetText();

            var dataElm = new ContainerElement(
                ContainerElementStyle.Stacked,
                new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, $"MUTATION: [{mutation.Mutation.Document.MutationDetails.Mutation.ToFullString()}]")));

            return Task.FromResult(new QuickInfoItem(lineSpan, dataElm));
        }

        public void Dispose()
        {
            MutationCodeHighlightHandler.OnMutationHighlightUpdate -= MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        private void MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(object sender, IList<MutationHighlight> e)
        {
            _mutations = new List<MutationHighlight>(e);
        }

        private bool InsideSpan(int position, ITextSnapshotLine line)
        {
            return position >= line.Start && position <= line.Start + line.Length;
        }
    }
}
