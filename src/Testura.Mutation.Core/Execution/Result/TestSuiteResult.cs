using System;
using System.Collections.Generic;

namespace Testura.Mutation.Core.Execution.Result
{
    public class TestSuiteResult
    {
        public string Name { get; set; }

        public bool IsSuccess { get; set; }

        public IList<TestResult> TestResults { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public string InnerXml { get; set; }

        public static TestSuiteResult Error(string message, TimeSpan time)
        {
            return new TestSuiteResult
            {
                IsSuccess = false,
                Name = $"ERROR - {message}",
                TestResults = new List<TestResult>()
            };
        }
    }
}
