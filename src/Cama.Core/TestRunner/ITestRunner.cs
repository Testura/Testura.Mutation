using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestSuiteResult = Cama.Core.TestRunner.Result.TestSuiteResult;

namespace Cama.Core.TestRunner
{
    public interface ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <param name="testNames">fsdf</param>
        /// <param name="maxTime">fd</param>
        /// <returns>Results from the nunit remote tester</returns>
        Task<TestSuiteResult> RunTestsAsync(string dllPath, IList<string> testNames, TimeSpan maxTime);
    }
}