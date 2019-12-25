using System.Windows.Media;
using Dragablz;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MutationConfigWindowControl.
    /// </summary>
    public partial class MutationConfigWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MutationConfigWindowControl"/> class.
        /// </summary>
        public MutationConfigWindowControl()
        {
            RunDummyCode();
            InitializeComponent();

            var viewModel = DataContext as MutationConfigWindowViewModel;
            viewModel?.Initialize();
        }

        private void RunDummyCode()
        {
            // Need this or we will get dll problems later on..
            ShadowAssist.SetShadowDepth(this, ShadowDepth.Depth0);
            var hue = new Hue("Dummy", Colors.AliceBlue, Colors.AntiqueWhite);
            var o = new TabablzControl();
        }
    }
}