using Prism.Mvvm;
using Testura.Mutation.Core;

namespace Testura.Mutation.VsExtension.Models
{
    public class MutationOperatorGridItem : BindableBase
    {
        private bool _isSelected;
        private MutationOperators _mutationOperator;
        private string _description;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public MutationOperators MutationOperator
        {
            get => _mutationOperator;
            set => SetProperty(ref _mutationOperator, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
