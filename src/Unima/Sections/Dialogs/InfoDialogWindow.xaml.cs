using System.Windows;

namespace Unima.Sections.Dialogs
{
    /// <summary>
    /// Interaction logic for InfoDialogWindow.xaml
    /// </summary>
    public partial class InfoDialogWindow : Window
    {
        public InfoDialogWindow(string message)
        {
            InitializeComponent();

            TxtMessage.Text = message;
        }

        private void BtnCloseOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnOkOnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
