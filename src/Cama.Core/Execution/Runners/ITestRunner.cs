using System;
using System.Threading.Tasks;
using TestSuiteResult = Cama.Core.Execution.Result.TestSuiteResult;

namespace Cama.Core.Execution.Runners
{
    public interface ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <param name="maxTime">fd</param>
        /// <returns>Results from the nunit remote tester</returns>
        Task<TestSuiteResult> RunTestsAsync(string dllPath, TimeSpan maxTime);
    }
}