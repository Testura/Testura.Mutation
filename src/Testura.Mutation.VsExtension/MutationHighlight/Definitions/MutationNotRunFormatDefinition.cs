﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Testura.Mutation.VsExtension.MutationHighlight.Definitions
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/MutationNotRunFormatDefinition")]
    [UserVisible(true)]
    public class MutationNotRunFormatDefinition : MarkerFormatDefinition
    {
        public MutationNotRunFormatDefinition()
        {
            var color = new SolidColorBrush(Color.FromArgb(255, 60, 141, 197));
            color.Opacity = 0.25;
            Fill = color;
            Border = new Pen(Brushes.Gray, 1.0);
            DisplayName = "Highlight Word";
            ZOrder = 5;
        }
    }
}
