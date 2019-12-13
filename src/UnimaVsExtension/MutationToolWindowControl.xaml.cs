using EnvDTE;
using MediatR;

namespace UnimaVsExtension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MutationToolWindowControl.
    /// </summary>
    public partial class MutationToolWindowControl : UserControl
    {
        private readonly MutationToolWindowControlViewModel _mutationToolWindowControlViewModel;

        public MutationToolWindowControl(MutationToolWindowControlViewModel mutationToolWindowControlViewModel)
        {
            _mutationToolWindowControlViewModel = mutationToolWindowControlViewModel;
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _mutationToolWindowControlViewModel.Do();

            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "MutationToolWindow");
        }

        public void Initialize(DTE getService)
        {
            _mutationToolWindowControlViewModel.Initialize(getService);
        }
    }
}