using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Prism.Mvvm;
using Testura.Mutation.VsExtension.Models;

namespace Testura.Mutation.VsExtension.MutationHighlight.Glyph.Dialog
{
    public class MutationCodeHighlightInfoDialogViewModel : BindableBase
    {
        private MutationHighlight _mutationHighlight;
        private string _status;
        private SideBySideDiffModel _diff;

        public MutationCodeHighlightInfoDialogViewModel(MutationHighlight mutationHighlight)
        {
            _mutationHighlight = mutationHighlight;
            Status = "Waiting..";

            if (_mutationHighlight.CompilationResult != null && !_mutationHighlight.CompilationResult.IsSuccess)
            {
                Status = "Failed to compile.";
                return;
            }

            if (!string.IsNullOrEmpty(_mutationHighlight.UnexpectedError))
            {
                Status = $"Unexpected error: {_mutationHighlight.UnexpectedError}";
                return;
            }

            if (_mutationHighlight.Status == TestRunDocument.TestRunStatusEnum.CompleteAndKilled)
            {
                Status = "Mutation was killed.";
            }

            if (_mutationHighlight.Status == TestRunDocument.TestRunStatusEnum.CompleteAndSurvived)
            {
                Status = "Mutation survived.";
            }

            var diffBuilder = new SideBySideDiffBuilder(new Differ());
            Diff = diffBuilder.BuildDiffModel(_mutationHighlight.OriginalText ?? string.Empty, _mutationHighlight.MutationText ?? string.Empty);
        }

        public MutationHighlight MutationHighlight
        {
            get => _mutationHighlight;
            set => SetProperty(ref _mutationHighlight, value);
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
