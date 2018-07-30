using System.Windows.Controls;
using Cama.Core.Models;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class FileDetailsView : TabItem
    {
        public FileDetailsView(MFile file)
        {
            InitializeComponent();

            var dataContext = DataContext as FileDetailsViewModel;
            dataContext.Initialize(file);
        }
    }
}
