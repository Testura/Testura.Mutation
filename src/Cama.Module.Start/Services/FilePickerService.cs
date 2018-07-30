using Microsoft.Win32;

namespace Cama.Module.Start.Services
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
    }
}
