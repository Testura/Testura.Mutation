using System.Threading.Tasks;
using TestSuiteResult = Unima.Core.Execution.Result.TestSuiteResult;

namespace Unima.Core.Execution.Runners
{
    public interface ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <returns>Results from the nunit remote tester</returns>
        Task<TestSuiteResult> RunTestsAsync(string dllPath);
    }
}