namespace Unima.VsExtension.Sections.Config
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
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
        }
    }
}