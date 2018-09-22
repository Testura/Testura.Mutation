using Cama.Core.Solution;

namespace Cama.Models
{
    public class ProjectListItem
    {
        public ProjectListItem(SolutionProjectInfo projectInfo, bool isSelected)
        {
            ProjectInfo = projectInfo;
            IsSelected = isSelected;
        }

        public SolutionProjectInfo ProjectInfo { get; set; }

        public bool IsSelected { get; set; }
    }
}
