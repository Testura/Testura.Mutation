using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit;

namespace Testura.Module.TestRunner.Result
{
    public class NUnitTestSuiteResult
    {
        private readonly string _innerXml;

        /// <summary>
        /// Gets the name of the test suite
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets if the test suite run was a success
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Gets the detailed result of this test suite
        /// </summary>
        public IList<NUnitTestCaseResult> TestResults { get; }

        public NUnitTestSuiteResult(string name, IList<NUnitTestCaseResult> results, string innerXml)
        {
            _innerXml = innerXml;
            Name = name;
            TestResults = results;
            IsSuccess = TestResults.All(t => t.IsSuccess);
        }
    }
}
