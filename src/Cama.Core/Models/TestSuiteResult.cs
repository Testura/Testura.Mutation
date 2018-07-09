using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Models
{
    public class TestSuiteResult
    {
        private readonly string _innerXml;

        public TestSuiteResult(string name, IList<TestResult> results, string innerXml)
        {
            _innerXml = innerXml;
            Name = name;
            TestResults = results;
            IsSuccess = TestResults.All(t => t.IsSuccess);
        }

        /// <summary>
        /// Gets the name of the test suite
        /// </summary>
        public string Name { get; private set; }

        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the detailed result of this test suite
        /// </summary>
        public IList<TestResult> TestResults { get; }
    }
}
