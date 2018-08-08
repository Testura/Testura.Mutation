using System.Windows.Controls;
using Cama.Core.Models.Mutation;
using Cama.Module.Mutation.Common.Avalon;

namespace Cama.Module.Mutation.Sections.Details
{
    /// <summary>
    /// Interaction logic for MutationDetailsView
    /// </summary>
    public partial class MutationDetailsView : TabItem
    {
        public MutationDetailsView(MutatedDocument document)
        {
            InitializeComponent();

            var dataContext = DataContext as MutationDetailsViewModel;
            dataContext.Initialize(document);

            /*
            textEditor.TextArea.TextView.LineTransformers.Add(new ChangeColorizer());
            */
        }
    }
}
