using System.Windows;
using Cama.Sections.Dialogs;

namespace Cama.Helpers.Displayers
{
    public static class ErrorDialogDisplayer
    {
        public static void ShowErrorDialog(string title, string message, string details = "")
        {
            var dialog = new ErrorDialogWindow(message, details, title)
            {
                ShowInTaskbar = false,
                Owner = Application.Current.MainWindow
            };

            dialog.ShowDialog();
        }
    }
}
