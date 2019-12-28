using System;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.TestRunner.Console.DotNet;
using Testura.Mutation.TestRunner.Console.NUnit;
using Testura.Mutation.TestRunner.Console.XUnit;

namespace Testura.Mutation.TestRunner
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
