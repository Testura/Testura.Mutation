using System.Runtime.Serialization;
using System.Windows.Forms;
using Testura.Mutation.Wpf.Shared.Extensions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Testura.Mutation.Helpers
{
    public class FilePicker
    {
        public enum Filter
        {
            [EnumMember(Value = "sln files (*.sln)|*.sln")]
            Solution,

            [EnumMember(Value = "json files (*.json)|*.json")]
            Project,

            [EnumMember(Value = "Testura.Mutation files (*.Testura.Mutation)|*.Testura.Mutation")]
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
