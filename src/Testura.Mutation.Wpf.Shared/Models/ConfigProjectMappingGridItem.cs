using System.ComponentModel;

namespace Testura.Mutation.Wpf.Shared.Models
{
    public class ConfigProjectMappingGridItem : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
