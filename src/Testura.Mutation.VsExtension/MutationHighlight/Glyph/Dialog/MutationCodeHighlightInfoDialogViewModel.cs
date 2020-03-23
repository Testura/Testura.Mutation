using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Prism.Mvvm;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog
{
    public class MutationCodeHighlightInfoDialogViewModel : BindableBase
    {
        private MutationRunItem _mutationRunItem;
        private string _status;
        private SideBySideDiffModel _diff;

        public MutationCodeHighlightInfoDialogViewModel(MutationRunItem mutationRunItem)
        {
            _mutationRunItem = mutationRunItem;
            Status = mutationRunItem.InfoText;

            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(
                _mutationRunItem.Document.MutationDetails.Original.ToFullString() ?? string.Empty,
                _mutationRunItem.Document.MutationDetails.Mutation.ToFullString() ?? string.Empty);
        }

        public MutationRunItem MutationRunItem
        {
            get => _mutationRunItem;
            set => SetProperty(ref _mutationRunItem, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public SideBySideDiffModel Diff
        {
            get => _diff;
            set => SetProperty(ref _diff, value);
        }
    }
}
