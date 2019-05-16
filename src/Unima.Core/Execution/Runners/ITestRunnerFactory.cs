namespace Unima.Core.Execution.Runners
{
    public interface ITestRunnerFactory
    {
        ITestRunner CreateTestRunner(string testRunnerName, string dotNetPath);
    }
}
