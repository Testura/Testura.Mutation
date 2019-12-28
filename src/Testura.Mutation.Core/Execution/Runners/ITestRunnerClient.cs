using System;
using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Core.Execution.Result;

namespace Testura.Mutation.Core.Execution.Runners
{
    public interface ITestRunnerClient
    {
        Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, string dotNetPath, TimeSpan maxTime, CancellationToken cancellationToken = default(CancellationToken));
    }
}
