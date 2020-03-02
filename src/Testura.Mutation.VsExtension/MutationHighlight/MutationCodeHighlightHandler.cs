using System;
using System.Collections.Generic;
using System.Linq;
using TestRunDocument = Testura.Mutation.VsExtension.Models.TestRunDocument;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public static class MutationCodeHighlightHandler
    {
        public static event EventHandler<IList<MutationHighlight>> OnMutationHighlightUpdate;

        public static IList<MutationHighlight> MutationHighlights { get; private set; }

        public static void UpdateMutationHighlightList(IEnumerable<MutationHighlight> mutationHightlights)
        {
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHighlight>(mutationHightlights));
        }

        public static void UpdateMutationHighlightList(IEnumerable<TestRunDocument> testRunDocument)
        {
            MutationHighlights = testRunDocument.Select(m =>
                new MutationHighlight
                {
                    Id = m.Document.Id,
                    UnexpectedError = m.Result?.UnexpectedError,
                    CompilationResult = m.Result?.CompilationResult,
                    FailedTests = m.Result?.FailedTests,
                    FilePath = m.Document.FilePath,
                    Line = m.Document.MutationDetails.Location.GetLineNumber(),
                    Start = m.Document.MutationDetails.Orginal.FullSpan.Start,
                    Length = m.Document.MutationDetails.Orginal.FullSpan.Length,
                    Status = m.Status,
                    MutationText = m.Document.MutationDetails.Mutation.ToFullString(),
                    OriginalText = m.Document.MutationDetails.Orginal.ToFullString()
                }).ToList();

            UpdateMutationHighlightList(MutationHighlights);
        }

        public static void ClearHighlights()
        {
            MutationHighlights?.Clear();
            OnMutationHighlightUpdate?.Invoke(typeof(MutationCodeHighlightHandler), new List<MutationHighlight>());
        }
    }
}
