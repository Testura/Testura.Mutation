using System;
using Cama.Core.Execution.Runners;
using Cama.TestRunner.NUnit;

namespace Cama.TestRunner
{
    public class TestRunnerFactory : ITestRunnerFactory
    {
        public const string NUnit = "nunit";

        public ITestRunner CreateTestRunner(string testRunnerName)
        {
            if (testRunnerName.Equals(NUnit, StringComparison.InvariantCultureIgnoreCase))
            {
                return new NUnitTestRunner();
            }

            throw new ArgumentException($"Could not find any test runner with the name {testRunnerName}.");
        }
    }
}
