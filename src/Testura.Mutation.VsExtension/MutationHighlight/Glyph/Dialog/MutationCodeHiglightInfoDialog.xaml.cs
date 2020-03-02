using Microsoft.VisualStudio.PlatformUI;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog
{
    /// <summary>
    /// Interaction logic for MutationCodeHiglightInfoDialog.xaml.
    /// </summary>
    public partial class MutationCodeHiglightInfoDialog : DialogWindow
    {
        public MutationCodeHiglightInfoDialog(MutationHighlight mutationHighlight)
        {
            DataContext = new MutationCodeHighlightInfoDialogViewModel(mutationHighlight);
            InitializeComponent();
        }
    }
}
