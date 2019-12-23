using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Unima.VsExtension.MutationHighlight
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/MutationCodeHighlightFormatDefinition")]
    [UserVisible(true)]
    public class MutationCodeHighlightFormatDefinition : MarkerFormatDefinition
    {
        public MutationCodeHighlightFormatDefinition()
        {
            var orange = Brushes.IndianRed.Clone();
            orange.Opacity = 0.25;
            Fill = orange;
            Border = new Pen(Brushes.Gray, 1.0);
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}
