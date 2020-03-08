using System.Windows;
using System.Windows.Controls;
using Testura.Mutation.VsExtension.Models;
using Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph
{
    /// <summary>
    /// Interaction logic for MutationCodeHighlightGlyph.xaml.
    /// </summary>
    public partial class MutationCodeHighlightGlyph : UserControl
    {
        private readonly MutationHighlight _mutationHighlight;

        public MutationCodeHighlightGlyph(MutationHighlight mutationHighlight)
        {
            _mutationHighlight = mutationHighlight;
            InitializeComponent();
        }

        public MutationRunItem.TestRunStatusEnum Status => _mutationHighlight.Mutation.Status;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var documentationControl = new MutationCodeHiglightInfoDialog(_mutationHighlight.Mutation);
            documentationControl.ShowDialog();
        }
    }
}
