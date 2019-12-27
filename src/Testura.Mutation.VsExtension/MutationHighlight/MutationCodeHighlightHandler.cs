using System;
using System.Collections.Generic;
using System.Linq;
using Testura.Mutation.Wpf.Shared.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public static class MutationCodeHighlightHandler
    {
        public static event EventHandler<IList<MutationHightlight>> OnMutationHighlightUpdate;

        public static IList<MutationHightlight> MutationHighlights { get; private set; }

        public static void UpdateMutationHighlightList(IEnumerable<MutationHightlight> mutationHightlights)
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>(mutationHightlights));
        }

        public static void UpdateMutationHighlightList(IEnumerable<TestRunDocument> testRunDocument)
        {
            MutationHighlights = testRunDocument.Select(m =>
                new MutationHightlight
                {
                    FilePath = m.Document.FilePath,
                    Line = m.Document.MutationDetails.Location.GetLineNumber(),
                    Start = m.Document.MutationDetails.Orginal.FullSpan.Start,
                    Length = m.Document.MutationDetails.Orginal.FullSpan.Length,
                    Status = m.Status,
                    MutationText = m.Document.MutationDetails.Mutation.ToFullString()
                }).ToList();

            UpdateMutationHighlightList(MutationHighlights);
        }

        public static void ClearHighlights()
        {
            MutationHighlights.Clear();
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>());
        }
    }
}
