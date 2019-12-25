using System;
using System.Collections.Generic;
using System.Linq;
using Unima.Wpf.Shared.Models;

namespace Unima.VsExtension.MutationHighlight
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
                    Length = m.Document.MutationDetails.Orginal.FullSpan.Length
                }));
        }

        public void ClearHighlights()
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>());
        }
    }
}
