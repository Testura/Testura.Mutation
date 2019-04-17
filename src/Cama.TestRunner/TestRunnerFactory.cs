using System;
using Cama.Core.Execution.Runners;
using Cama.TestRunner.DotNet;
using Cama.TestRunner.NUnit;
using Cama.TestRunner.XUnit;

namespace Cama.TestRunner
{
    public class TestRunnerFactory : ITestRunnerFactory
    {
        public const string NUnit = "nunit";
        public const string XUnit = "xunit";
        public const string DotNet = "dotnet";

        public ITestRunner CreateTestRunner(string testRunnerName)
        {
            if (testRunnerName.Equals(NUnit, StringComparison.InvariantCultureIgnoreCase))
            {
                return new NUnitTestRunner();
            }

            if (testRunnerName.Equals(XUnit, StringComparison.InvariantCultureIgnoreCase))
            {
                return new XUnitTestRunner();
            }

            if (testRunnerName.Equals(DotNet, StringComparison.InvariantCultureIgnoreCase))
            {
                return new DotNetTestRunner();
            }

            throw new ArgumentException($"Could not find any test runner with the name {testRunnerName}.");
        }
    }
}
