using System.Windows.Media;
using Dragablz;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualStudio.PlatformUI;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    /// <summary>
    /// Interaction logic for MutationConfigWindowControl.
    /// </summary>
    public partial class MutationConfigWindowControl : DialogWindow
    {
        public MutationConfigWindowControl(MutationConfigWindowViewModel mutationConfigWindowViewModel)
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;

            DataContext = mutationConfigWindowViewModel;

            RunDummyCode();
            InitializeComponent();

            mutationConfigWindowViewModel.Initialize();
        }

        private void RunDummyCode()
        {
            // Need this or we will get dll problems later on..
            ShadowAssist.SetShadowDepth(this, ShadowDepth.Depth0);
            var hue = new Hue("Dummy", Colors.AliceBlue, Colors.AntiqueWhite);
            var o = new TabablzControl();
        }

        private void UpdateConfigButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dataContext = DataContext as MutationConfigWindowViewModel;
            if (dataContext == null || dataContext.UpdateConfig())
            {
                DialogResult = true;
                Close();
            }
        }
    }
}