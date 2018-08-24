using System.Windows.Controls;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class FileDetailsView : TabItem
    {
        public FileDetailsView(MFile file, CamaRunConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as FileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
