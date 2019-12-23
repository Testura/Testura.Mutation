using System;
using System.Collections.Generic;

namespace Unima.VsExtension.MutationHighlight
{
    public class MutationCodeHighlightHandler
    {
        public static event EventHandler<IList<MutationHightlight>> OnMutationHighlightUpdate;

        public void UpdateMutationHighlightList(IList<MutationHightlight> mutationHightlights)
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHightlight>(mutationHightlights));
        }
    }
}
