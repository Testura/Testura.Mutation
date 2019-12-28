using Testura.Mutation.Sections.Dialogs;

namespace Testura.Mutation.Helpers.Displayers
{
    public static class CommonDialogDisplayer
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

        public static bool ShowInfoDialog(string message)
        {
            var dialog = new InfoDialogWindow(message)
            {
                ShowInTaskbar = false,
                Owner = System.Windows.Application.Current.MainWindow
            };

            dialog.ShowDialog();

            return dialog.DialogResult.Value;
        }
    }
}
