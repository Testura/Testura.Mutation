using System.Windows.Controls;
using Testura.Mutation.Application;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Sections.MutationDetails
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationFileDetailsView : TabItem
    {
        public MutationFileDetailsView(FileMutationsModel file, TesturaMutationConfig config)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationFileDetailsViewModel;
            dataContext.Initialize(file, config);
        }
    }
}
