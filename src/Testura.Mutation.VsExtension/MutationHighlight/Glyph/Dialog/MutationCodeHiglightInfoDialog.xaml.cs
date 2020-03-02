using Microsoft.VisualStudio.PlatformUI;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog
{
    /// <summary>
    /// Interaction logic for MutationCodeHiglightInfoDialog.xaml.
    /// </summary>
    public partial class MutationCodeHiglightInfoDialog : DialogWindow
    {
        public MutationCodeHiglightInfoDialog(MutationHighlight mutationHighlight)
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;

            DataContext = new MutationCodeHighlightInfoDialogViewModel(mutationHighlight);
            InitializeComponent();
        }
    }
}
