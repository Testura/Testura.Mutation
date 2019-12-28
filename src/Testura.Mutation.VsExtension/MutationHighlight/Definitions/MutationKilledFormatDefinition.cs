using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Testura.Mutation.VsExtension.MutationHighlight.Definitions
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/MutationKilledFormatDefinition")]
    [UserVisible(true)]
    public class MutationKilledFormatDefinition : MarkerFormatDefinition
    {
        public MutationKilledFormatDefinition()
        {
            var color = Brushes.Green.Clone();
            color.Opacity = 0.25;
            Fill = color;
            Border = new Pen(Brushes.Gray, 1.0);
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}
