using System;
using System.Threading.Tasks;
using Unima.Core.Execution.Result;

namespace Unima.Core.Execution.Runners
{
    public interface ITestRunnerClient
    {
        Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, string dotNetPath, TimeSpan maxTime);
    }
}
