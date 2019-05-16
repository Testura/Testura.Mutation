using System.Windows.Controls;
using Unima.Application;
using Unima.Core.Config;

namespace Unima.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationFileDetailsView : TabItem
    {
        public MutationFileDetailsView(FileMutationsModel file, UnimaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationFileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
