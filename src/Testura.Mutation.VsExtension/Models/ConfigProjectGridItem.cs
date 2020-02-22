using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Testura.Mutation.VsExtension.Models
{
    public class ConfigProjectGridItem : BindableBase
    {
        private bool _isTestProject;
        private bool _isIgnored;
        private string _name;
        private ObservableCollection<ConfigProjectMappingGridItem> _projectMapping;

        public bool IsTestProject
        {
            get => _isTestProject;
            set => SetProperty(ref _isTestProject, value);
        }

        public bool IsIgnored
        {
            get => _isIgnored;
            set => SetProperty(ref _isIgnored, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<ConfigProjectMappingGridItem> ProjectMapping
        {
            get => _projectMapping;
            set => SetProperty(ref _projectMapping, value);
        }
    }
}
