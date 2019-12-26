using System;
using System.Collections.Generic;
using System.Linq;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightHandler
    {
        public static event EventHandler<IList<MutationHightlight>> OnMutationHighlightUpdate;

        public void UpdateMutationHighlightList(IEnumerable<MutationHightlight> mutationHightlights)
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>(mutationHightlights));
        }

        public void UpdateMutationHighlightList(IEnumerable<TestRunDocument> testRunDocument)
        {
            UpdateMutationHighlightList(testRunDocument.Select(m =>
                new MutationHightlight
                {
                    FilePath = m.Document.FilePath,
                    Line = m.Document.MutationDetails.Location.GetLineNumber(),
                    Start = m.Document.MutationDetails.Orginal.FullSpan.Start,
                    Length = m.Document.MutationDetails.Orginal.FullSpan.Length,
                    Status = m.Status,
                    MutationText = m.Document.MutationDetails.Mutation.ToFullString()
                }));
        }

        public void ClearHighlights()
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>());
        }
    }
}
