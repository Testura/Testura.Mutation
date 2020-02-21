using Prism.Mvvm;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.VsExtension.Models
{
    public class ProjectListItem : BindableBase
    {
        private SolutionProjectInfo _solutionProjectInfo;
        private bool _isSelected;

        public ProjectListItem(SolutionProjectInfo projectInfo, bool isSelected)
        {
            ProjectInfo = projectInfo;
            IsSelected = isSelected;
        }

        public SolutionProjectInfo ProjectInfo
        {
            get => _solutionProjectInfo;
            set => SetProperty(ref _solutionProjectInfo, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
