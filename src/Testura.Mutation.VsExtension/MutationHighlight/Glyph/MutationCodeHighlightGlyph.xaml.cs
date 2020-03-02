using System.Windows;
using System.Windows.Controls;
using Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph
{
    /// <summary>
    /// Interaction logic for MutationCodeHighlightGlyph.xaml.
    /// </summary>
    public partial class MutationCodeHighlightGlyph : UserControl
    {
        public MutationCodeHighlightGlyph()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var documentationControl = new MutationCodeHiglightInfoDialog();
            documentationControl.ShowDialog();
        }
    }
}
