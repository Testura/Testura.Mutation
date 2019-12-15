using System.Windows.Media;
using Dragablz;
using EnvDTE;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.Threading;

namespace Unima.VsExtension.Sections.Config
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for UnimaConfigWindowControl.
    /// </summary>
    public partial class UnimaConfigWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnimaConfigWindowControl"/> class.
        /// </summary>
        public UnimaConfigWindowControl()
        {
            InitializeComponent();

            // Need this or we will get dll problems later on..
            ShadowAssist.SetShadowDepth(this, ShadowDepth.Depth0);
            var hue = new Hue("Dummy", Colors.AliceBlue, Colors.AntiqueWhite);
            var o = new TabablzControl();
        }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            ((UnimaConfigWindowViewModel)DataContext).Initialize(dte, joinableTaskFactory);
        }
    }
}