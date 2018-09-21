using System.Windows.Controls;
using Cama.Core.Config;
using Cama.Infrastructure.Models;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class FileDetailsView : TabItem
    {
        public FileDetailsView(FileMutationsModel file, CamaConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as FileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
