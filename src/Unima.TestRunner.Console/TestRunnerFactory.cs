using System;
using Unima.Core.Execution.Runners;
using Unima.TestRunner.Console.DotNet;
using Unima.TestRunner.Console.NUnit;
using Unima.TestRunner.Console.XUnit;

namespace Unima.TestRunner
{
    public class TestRunnerFactory : ITestRunnerFactory
    {
        public const string NUnit = "nunit";
        public const string XUnit = "xunit";
        public const string DotNet = "dotnet";

        public ITestRunner CreateTestRunner(string testRunnerName, TimeSpan maxTime, string dotNetPath)
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
                return new DotNetTestRunner(dotNetPath, maxTime);
            }

            throw new ArgumentException($"Could not find any test runner with the name {testRunnerName}.");
        }
    }
}
