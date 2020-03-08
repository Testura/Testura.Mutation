using System;
using System.Collections.Generic;
using System.Linq;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public static class MutationCodeHighlightHandler
    {
        public static event EventHandler<IList<MutationHighlight>> OnMutationHighlightUpdate;

        public static IList<MutationHighlight> MutationHighlights { get; private set; }

        public static void UpdateMutationHighlightList(IEnumerable<MutationHighlight> mutationHightlights)
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHighlight>(mutationHightlights));
        }

        public static void UpdateMutationHighlightList(IEnumerable<MutationRunItem> mutations)
        {
            MutationHighlights = mutations.Select(m => new MutationHighlight { Mutation = m }).ToList();

            UpdateMutationHighlightList(MutationHighlights);
        }

        public static void ClearHighlights()
        {
            MutationHighlights?.Clear();
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHighlight>());
        }
    }
}
