using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.TestRunner.Console.DotNet;

namespace Testura.Mutation.Tests.TestRunner.DotNet
{
    [TestFixture]
    public class DotNetTestRunnerXunitTests
    {
        [Test]
        public async Task DotNetTestRunnerWithXUnit()
        {
            var binPath = TestContext.CurrentContext.TestDirectory;

            var m = binPath.Substring(0, binPath.IndexOf("bin"));
            var testDllPath = Path.Combine(m, "TestRunner", "Samples", "Xunit", "PassingTests", "XUnitTestProject.dll");

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsTrue(result.IsSuccess, "Test result should be successful");
            Assert.AreEqual(2, result.TestResults.Count, "Wrong test result count");
            Assert.AreEqual(2, result.TestResults.Count(t => t.IsSuccess), "Should find two test that pass");
        }

        [Test]
        public async Task DotNetTestRunnerWithXUnitFail()
        {
            var binPath = TestContext.CurrentContext.TestDirectory;

            var m = binPath.Substring(0, binPath.IndexOf("bin"));
            var testDllPath = Path.Combine(m, "TestRunner", "Samples", "Xunit", "FailingTests", "XUnitTestProject.dll");

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsFalse(result.IsSuccess, "Test result should not be successful");
            Assert.AreEqual(2, result.TestResults.Count, "Wrong test result count");
            Assert.IsTrue(result.TestResults.Any(t => t.IsSuccess), "Should find one test that pass");
            Assert.IsTrue(result.TestResults.Any(t => !t.IsSuccess), "Should find one test that fail");
        }

        [Test]
        public async Task DotNetTestRunnerWithXUnitErrors()
        {
            var binPath = TestContext.CurrentContext.TestDirectory;

            var m = binPath.Substring(0, binPath.IndexOf("bin"));
            var testDllPath = Path.Combine(m, "TestRunner", "Samples", "Xunit", "WithErrors", "XUnitTestProject.dll");

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsFalse(result.IsSuccess, "Test result should not be successful");
            Assert.IsTrue(result.Name.StartsWith("ERROR"));
            StringAssert.Contains("An assembly specified in the application dependencies manifest (XUnitTestProject.deps.json) was not found", result.Name);
        }
    }
}
