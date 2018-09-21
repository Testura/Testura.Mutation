using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report.Cama
{
    public class CamaReportMutationItem
    {
        public CamaReportMutationItem()
        {
        }

        public CamaReportMutationItem(MutationDocumentResult mutation)
        {
            var document = mutation.Document;
            ProjectName = document.ProjectName;
            FileName = document.FileName;
            Orginal = document.MutationInfo.Orginal.ToFullString();
            FullOrginal = document.MutationInfo.FullOrginal.ToFullString();
            Mutation = document.MutationInfo.Mutation.ToFullString();
            FullMutation = document.MutationInfo.FullMutation.ToFullString();
            Survived = mutation.TestResult?.IsSuccess ?? false;
            CompileResult = mutation.CompilerResult;
            TestsRun = mutation.TestResult?.TestResults.Count ?? 0;
            FailedTests = mutation.TestResult?.TestResults.Where(t => !t.IsSuccess).ToList() ?? new List<TestResult>();
        }

        public string ProjectName { get; set; }

        public string FileName { get; set; }

        public string Orginal { get; set; }

        public string FullOrginal { get; set; }

        public string Mutation { get; set; }

        public string FullMutation { get; set; }

        public bool Survived { get; set; }

        public CompilationResult CompileResult { get; set; }

        public int TestsRun { get; set; }

        public IList<TestResult> FailedTests { get; set; }
    }
}
