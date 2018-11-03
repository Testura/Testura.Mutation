using System.Windows.Controls;
using Cama.Application;
using Cama.Core;

namespace Cama.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationFileDetailsView : TabItem
    {
        public MutationFileDetailsView(FileMutationsModel file, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationFileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
