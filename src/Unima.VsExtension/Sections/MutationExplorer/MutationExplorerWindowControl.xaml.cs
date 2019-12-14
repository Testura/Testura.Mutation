using System.Windows.Controls;
using System.Windows.Media;
using Dragablz;
using EnvDTE;
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
            this.InitializeComponent();
            ShadowAssist.SetShadowDepth(this, ShadowDepth.Depth0);
            var hue = new Hue("Dummy", Colors.AliceBlue, Colors.AntiqueWhite);
            var o = new TabablzControl();
        }

        public void Initialize(DTE dte)
        {
            ((MutationExplorerWindowViewModel)DataContext).Initialize(dte);
        }
    }
}