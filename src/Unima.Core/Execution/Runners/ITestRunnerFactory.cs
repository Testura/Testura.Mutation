using System;

namespace Unima.Core.Execution.Runners
{
    public interface ITestRunnerFactory
    {
        ITestRunner CreateTestRunner(string testRunnerName, TimeSpan maxTime, string dotNetPath);
    }
}
