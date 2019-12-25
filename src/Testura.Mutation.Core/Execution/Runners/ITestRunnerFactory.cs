using System;

namespace Testura.Mutation.Core.Execution.Runners
{
    public interface ITestRunnerFactory
    {
        ITestRunner CreateTestRunner(string testRunnerName, TimeSpan maxTime, string dotNetPath);
    }
}
