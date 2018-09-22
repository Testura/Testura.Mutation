using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Cama.Services
{
    public class FilePickerService
    {
        public string PickFile()
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "sln files (*.sln)|*.sln"
            };

            fileDialog.ShowDialog();
            return fileDialog.FileName;
        }

        public string PickDirectory()
        {
            var fileDialog = new FolderBrowserDialog();
            fileDialog.ShowDialog();
            return fileDialog.SelectedPath;
        }
    }
}
