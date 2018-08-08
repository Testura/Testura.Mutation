using System.Drawing;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Brushes = System.Windows.Media.Brushes;

namespace Cama.Module.Mutation.Common.Avalon
{
    public class ChangeColorizer : DocumentColorizingTransformer
    {
        public int StartIndex { get; set; }

        public int StopIndex { get; set; }

        protected override void ColorizeLine(DocumentLine line)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf("<", start)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + 1, // endOffset
                    (VisualLineElement element) => { element.TextRunProperties.SetBackgroundBrush(Brushes.Yellow); });
                start = index + 1; // search for next occurrence
            }
        }
    }
}
