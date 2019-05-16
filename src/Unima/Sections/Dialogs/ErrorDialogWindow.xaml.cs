using System.Windows;

namespace Unima.Sections.Dialogs
{
    /// <summary>
    /// Interaction logic for ErrorDialogWindow.xaml
    /// </summary>
    public partial class ErrorDialogWindow : Window
    {
        public ErrorDialogWindow(string message, string details = "", string title = "Unexpected error")
        {
            InitializeComponent();

            Title = title;
            TxtMessage.Text = message;
            TxtDetails.Text = details;
            ExpDetails.Visibility = string.IsNullOrEmpty(details) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void BtnCloseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
