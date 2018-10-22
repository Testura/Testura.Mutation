using System.Runtime.Serialization;
using System.Windows.Forms;
using Cama.Helpers.Extensions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Cama.Helpers
{
    public class FilePicker
    {
        public enum Filter
        {
            [EnumMember(Value = "sln files (*.sln)|*.sln")]
            Solution,

            [EnumMember(Value = "json files (*.json)|*.json")]
            Project,

            [EnumMember(Value = "cama files (*.cama)|*.cama")]
            Report
        }

        public string PickFile(Filter filter)
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = filter.GetValue()
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
