using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Testura.Mutation.VsExtension.MutationHighlight.Definitions
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/MutationUnknownErrorFormatDefinition")]
    [UserVisible(true)]
    public class MutationUnknownErrorFormatDefinition : MarkerFormatDefinition
    {
        public MutationUnknownErrorFormatDefinition()
        {
            var color = Brushes.MediumPurple.Clone();
            color.Opacity = 0.25;
            Fill = color;
            Border = new Pen(Brushes.Gray, 1.0);
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}
