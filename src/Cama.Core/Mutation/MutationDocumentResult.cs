using System.Collections.Generic;
using System.Linq;
using Cama.Core.Models;
using Cama.Core.Mutation.Compilation;
using Cama.Core.TestRunner.Result;

namespace Cama.Core.Mutation.Models
{
    public class MutationDocumentResult
    {
        public MutationDocumentResult()
        {
            FailedTests = new List<TestResult>();
        }

        public string ProjectName { get; set; }

        public string FileName { get; set; }

        public string Orginal { get; set; }

        public string FullOrginal { get; set; }

        public string Mutation { get; set; }

        public string FullMutation { get; set; }

        public bool Survived => !FailedTests.Any() && CompilerResult.IsSuccess;

        public CompilationResult CompilerResult { get; set; }

        public int TestsRunCount { get; set; }

        public IList<TestResult> FailedTests { get; set; }

        public MutationLocationInfo Location { get; set; }

        public string MutationName { get; set; }
    }
}
