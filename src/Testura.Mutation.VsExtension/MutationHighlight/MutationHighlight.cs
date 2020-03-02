using System.Collections.Generic;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Result;
using TestRunDocument = Testura.Mutation.VsExtension.Models.TestRunDocument;

namespace Testura.Mutation.VsExtension.MutationHighlight
{
    public class MutationHighlight
    {
        public string FilePath { get; set; }

        public string MutationText { get; set; }

        public string OriginalText { get; set; }

        public int Line { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public TestRunDocument.TestRunStatusEnum Status { get; set; }

        public CompilationResult CompilationResult { get; set; }

        public IList<TestResult> FailedTests { get; set; }

        public string Id { get; set; }

        public string UnexpectedError { get; set; }
    }
}
