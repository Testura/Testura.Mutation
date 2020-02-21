using Prism.Mvvm;

namespace Testura.Mutation.VsExtension.Models
{
    public class ConfigProjectMappingGridItem : BindableBase
    {
        private string _name;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
