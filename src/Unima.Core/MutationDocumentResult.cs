using System;
using System.Collections.Generic;
using System.Linq;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Result;
using Unima.Core.Location;
using Unima.Core.Util;

namespace Unima.Core
{
    public class MutationDocumentResult
    {
        public MutationDocumentResult()
        {
            FailedTests = new List<TestResult>();
        }

        public Guid Id { get; set; }

        public string ProjectName { get; set; }

        public string FileName { get; set; }

        public string Orginal { get; set; }

        public string FullOrginal { get; set; }

        public string Mutation { get; set; }

        public string FullMutation { get; set; }

        public bool Survived => !FailedTests.Any() && (CompilationResult != null && CompilationResult.IsSuccess);

        public CompilationResult CompilationResult { get; set; }

        public int TestsRunCount { get; set; }

        public IList<TestResult> FailedTests { get; set; }

        public MutationLocationInfo Location { get; set; }

        public string MutationName { get; set; }

        public string UnexpectedError { get; set; }

        public string Hash { get; set; }

        public void GenerateHash()
        {
            Hash = HashHelper.CreateMD5Hash(
                ProjectName +
                FileName +
                Orginal +
                Mutation);
        }
    }
}
