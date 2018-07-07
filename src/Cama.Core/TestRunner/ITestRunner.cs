using System.Collections.Generic;
using Cama.Core.TestRunner.Result;

namespace Cama.Core.TestRunner
{
    public interface ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <param name="testNames">fsdf</param>
        /// <returns>Results from the nunit remote tester</returns>
        NUnitTestSuiteResult RunTests(string dllPath, IList<string> testNames);
    }
}