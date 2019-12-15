using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Dragablz;
using EnvDTE;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.Threading;

namespace Unima.VsExtension.Sections.MutationExplorer
{
    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationExplorerWindowControl : UserControl
    {
        public MutationExplorerWindowControl()
        {
            InitializeComponent();

            // Need this or we will get dll problems later on..
            ShadowAssist.SetShadowDepth(this, ShadowDepth.Depth0);
            var hue = new Hue("Dummy", Colors.AliceBlue, Colors.AntiqueWhite);
            var o = new TabablzControl();
        }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            ((MutationExplorerWindowViewModel)DataContext).Initialize(dte, joinableTaskFactory);
        }

        public void Initialize(DTE dte, JoinableTaskFactory joinableTaskFactory, IEnumerable<string> files)
        {
            ((MutationExplorerWindowViewModel)DataContext).Initialize(dte, joinableTaskFactory, files);
        }
    }
}