namespace Cama.Core.Execution.Runners
{
    public interface ITestRunnerFactory
    {
        ITestRunner CreateTestRunner(string testRunnerName);
    }
}
