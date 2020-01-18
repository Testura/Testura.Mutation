using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Testura.Mutation.Wpf.Shared.Models
{
    public class ConfigProjectGridItem : INotifyPropertyChanged
    {
#pragma warning disable 067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 067

        public bool IsTestProject { get; set; }

        public bool IsIgnored { get; set; }

        public string Name { get; set; }

        public ObservableCollection<ConfigProjectMappingGridItem> ProjectMapping { get; set; }
    }
}
