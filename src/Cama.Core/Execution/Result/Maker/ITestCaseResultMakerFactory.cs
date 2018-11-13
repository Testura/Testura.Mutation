namespace Cama.Core.Execution.Result.Maker
{
    public interface ITestCaseResultMakerFactory
    {
        ITestCaseResultMaker CreateTestCaseResultMaker(string testRunnerName);
    }
}
