using Unima.Sections.Dialogs;

namespace Unima.Helpers.Displayers
{
    public static class ErrorDialogDisplayer
    {
        public static void ShowErrorDialog(string title, string message, string details = "")
        {
            var dialog = new ErrorDialogWindow(message, details, title)
            {
                ShowInTaskbar = false,
                Owner = System.Windows.Application.Current.MainWindow
            };

            dialog.ShowDialog();
        }
    }
}
