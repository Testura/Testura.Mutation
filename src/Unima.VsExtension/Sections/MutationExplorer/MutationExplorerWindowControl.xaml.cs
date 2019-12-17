using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Dragablz;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationExplorerWindowControl : UserControl
    {
        public MutationExplorerWindowControl()
        {
            RunDummyCode();
            InitializeComponent();
        }

        public void Initialize()
        {
            var viewModel = DataContext as MutationExplorerWindowViewModel;
            viewModel?.Initialize();
        }

        public void Initialize(IEnumerable<string> files)
        {
            var viewModel = DataContext as MutationExplorerWindowViewModel;
            viewModel?.Initialize(files);
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            AfterTxt.ScrollToVerticalOffset(e.VerticalOffset);
            AfterTxt.ScrollToHorizontalOffset(e.HorizontalOffset);
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