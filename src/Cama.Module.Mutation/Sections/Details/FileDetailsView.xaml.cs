using System.Windows.Controls;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class FileDetailsView : TabItem
    {
        public FileDetailsView(MFile file, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as FileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
