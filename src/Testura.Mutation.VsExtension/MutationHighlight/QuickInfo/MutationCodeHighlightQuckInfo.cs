using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

namespace Testura.Mutation.VsExtension.MutationHighlight.QuickInfo
{
    public class MutationCodeHighlightQuckInfo : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _textBuffer;
        private IList<MutationHightlight> _mutations;

        public MutationCodeHighlightQuckInfo(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
            _mutations = new List<MutationHightlight>();

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
            var mutation = _mutations.FirstOrDefault(m => InsideSpan(m.Start, line) || InsideSpan(m.Start + m.Length, line));

            if (mutation == null)
            {
                return Task.FromResult<QuickInfoItem>(null);
            }

            var lineSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive);

            var text = triggerPoint.Value.GetContainingLine().GetText();

            var dataElm = new ContainerElement(
                ContainerElementStyle.Stacked,
                new ClassifiedTextElement(
                    new ClassifiedTextRun(PredefinedClassificationTypeNames.Keyword, $"MUTATION: [{mutation.MutationText}]")));

            return Task.FromResult(new QuickInfoItem(lineSpan, dataElm));
        }

        public void Dispose()
        {
            MutationCodeHighlightHandler.OnMutationHighlightUpdate -= MutationCodeHighlightHandlerOnOnMutationHighlightUpdate;
        }

        private void MutationCodeHighlightHandlerOnOnMutationHighlightUpdate(object sender, IList<MutationHightlight> e)
        {
            _mutations = e;
        }

        private bool InsideSpan(int position, ITextSnapshotLine line)
        {
            return position >= line.Start && position <= line.Start + line.Length;
        }
    }
}
