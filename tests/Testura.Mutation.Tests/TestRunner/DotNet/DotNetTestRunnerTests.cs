using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.TestRunner.Console.DotNet;

namespace Testura.Mutation.Tests.TestRunner.DotNet
{
    [TestFixture]
    public class DotNetTestRunnerTests
    {
        [TestCase("XUnit", "XUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllXunitTestPass_ShouldGetCorrectResult")]
        [TestCase("MsTest", "MsTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllMsTestPass_ShouldGetCorrectResult")]
        [TestCase("Nunit", "NUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllNunitTestPass_ShouldGetCorrectResult")]
        public async Task RunTestsAsync_WhenRunningAndAllTestPass_ShouldGetCorrectResult(string type, string dllName)
        {
            var testDllPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", type, "PassingTests", dllName);

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsTrue(result.IsSuccess, "Test result should be successful");
            Assert.AreEqual(2, result.TestResults.Count, "Wrong test result count");
            Assert.AreEqual(2, result.TestResults.Count(t => t.IsSuccess), "Should find two test that pass");
        }

        [TestCase("XUnit", "XUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndXunitTestFail_ShouldGetCorrectResult")]
        [TestCase("MsTest", "MsTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndMsTestFailt_ShouldGetCorrectResult")]
        [TestCase("Nunit", "NUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndNunitTestFailt_ShouldGetCorrectResult")]
        public async Task RunTestsAsync_WhenRunningAndSomeUnitTestFail_ShouldGetCorrectResult(string type, string dllName)
        {
            var testDllPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", type, "FailingTests", dllName);

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsFalse(result.IsSuccess, "Test result should not be successful");
            Assert.AreEqual(2, result.TestResults.Count, "Wrong test result count");
            Assert.IsTrue(result.TestResults.Any(t => t.IsSuccess), "Should find one test that pass");
            Assert.IsTrue(result.TestResults.Any(t => !t.IsSuccess), "Should find one test that fail");
        }

        [TestCase("XUnit", "XUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllXunitTestHaveUnknownErrors_ShouldGetCorrectResult")]
        [TestCase("MsTest", "MsTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllMsTestHaveUnknownErrors_ShouldGetCorrectResult")]
        [TestCase("Nunit", "NUnitTestProject.dll", TestName = "RunTestsAsync_WhenRunningAndAllNunitHaveUnknownErrors_ShouldGetCorrectResult")]
        public async Task RunTestsAsync_WhenRunningAndProjectCont_ShouldGetCorrectResult(string type, string dllName)
        {
            var testDllPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", type, "WithErrors", dllName);

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsFalse(result.IsSuccess, "Test result should not be successful");
            Assert.IsTrue(result.Name.StartsWith("ERROR"));
            StringAssert.Contains("was not found", result.Name);
        }

        [Test]
        public async Task RunTestsAsync_WhenRunningAndAllTestPassFromANonCoreProject_ShouldGetCorrectResult()
        {
            var testDllPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples", "NonCore", "NUnit", "PassingTests", "NunitTestProject.dll");

            var dotNetRunner = new DotNetTestRunner(null, TimeSpan.FromSeconds(5));
            var result = await dotNetRunner.RunTestsAsync(testDllPath);

            Assert.IsTrue(result.IsSuccess, "Test result should be successful");
            Assert.AreEqual(2, result.TestResults.Count, "Wrong test result count");
            Assert.AreEqual(2, result.TestResults.Count(t => t.IsSuccess), "Should find two test that pass");
        }
    }
}
