using System;
using System.Threading.Tasks;
using Cama.Core.Execution.Result;

namespace Cama.Core.Execution.Runners
{
    public interface ITestRunnerClient
    {
        Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, TimeSpan maxTime);
    }
}
