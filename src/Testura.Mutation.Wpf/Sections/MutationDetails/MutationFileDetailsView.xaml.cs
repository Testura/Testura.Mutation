using System.Windows.Controls;
using Testura.Mutation.Application;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Wpf.Sections.MutationDetails;

namespace Testura.Mutation.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView.
    /// </summary>
    public partial class MutationFileDetailsView : TabItem
    {
        public MutationFileDetailsView(FileMutationsModel file, MutationConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationFileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
