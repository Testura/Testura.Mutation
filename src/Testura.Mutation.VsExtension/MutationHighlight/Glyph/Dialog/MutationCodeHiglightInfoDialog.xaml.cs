using Microsoft.VisualStudio.PlatformUI;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog
{
    /// <summary>
    /// Interaction logic for MutationCodeHiglightInfoDialog.xaml.
    /// </summary>
    public partial class MutationCodeHiglightInfoDialog : DialogWindow
    {
        public MutationCodeHiglightInfoDialog(MutationRunItem mutationRunItem)
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;

            DataContext = new MutationCodeHighlightInfoDialogViewModel(mutationRunItem);
            InitializeComponent();
        }
    }
}
