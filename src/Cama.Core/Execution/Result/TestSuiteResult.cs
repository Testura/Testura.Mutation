using System;
using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Execution.Result
{
    public class TestSuiteResult
    {
        private readonly string _innerXml;

        public TestSuiteResult(string name, IList<TestResult> results, string innerXml, TimeSpan executionTime)
        {
            _innerXml = innerXml;
            Name = name;
            TestResults = results;
            IsSuccess = TestResults.All(t => t.IsSuccess);
            ExecutionTime = executionTime;
        }

        /// <summary>
        /// Gets the name of the test suite
        /// </summary>
        public string Name { get; }

        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the detailed result of this test suite
        /// </summary>
        public IList<TestResult> TestResults { get; }

        public TimeSpan ExecutionTime { get; }
    }
}
