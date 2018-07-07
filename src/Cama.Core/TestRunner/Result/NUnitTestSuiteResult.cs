using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.TestRunner.Result
{
    public class NUnitTestSuiteResult
    {
        private readonly string _innerXml;

        public NUnitTestSuiteResult(string name, IList<NUnitTestCaseResult> results, string innerXml)
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
        public IList<NUnitTestCaseResult> TestResults { get; }
    }
}
