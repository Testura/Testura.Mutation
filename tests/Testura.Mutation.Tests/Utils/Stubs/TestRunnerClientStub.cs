using System;
using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class TestRunnerClientStub : ITestRunnerClient
    {
        private readonly TestSuiteResult _testSuiteResult;

        public TestRunnerClientStub(TestSuiteResult testSuiteResult)
        {
            _testSuiteResult = testSuiteResult;
        }

        public Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, string dotNetPath, TimeSpan maxTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(_testSuiteResult);
        }
    }
}
