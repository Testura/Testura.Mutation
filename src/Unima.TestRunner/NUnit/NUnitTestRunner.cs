using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NUnit;
using NUnit.Engine;
using NUnit.Engine.Runners;
using NUnit.Engine.Services;
using TestSuiteResult = Unima.Core.Execution.Result.TestSuiteResult;

namespace Unima.TestRunner.NUnit
{
    public class NUnitTestRunner : Core.Execution.Runners.ITestRunner
    {
        public Task<TestSuiteResult> RunTestsAsync(string dllPath)
        {
            return Task.Run(() =>
            {
                var package = new TestPackage(dllPath);

                using (var engine = new TestEngine())
                {
                    engine.Services.Add(new SettingsService(false));
                    engine.Services.Add(new ExtensionService());
                    engine.Services.Add(new DriverService());
                    engine.Services.Add(new InProcessTestRunnerFactory());
                    engine.Services.Add(new ProjectService()); // +
                    engine.Services.Add(new RuntimeFrameworkService()); // +
                    engine.Services.Add(new TestFilterService()); // +
                    engine.Services.Add(new DomainManager()); // -
                    engine.Services.Add(new ResultService());
                    engine.Services.ServiceManager.StartServices();

                    using (global::NUnit.Engine.ITestRunner runner = engine.GetRunner(package))
                    {
                        try
                        {
                            var filter = TestFilter.Empty;
                            var result = runner.Run(new TestEventDispatcher(), filter);

                            if (result == null)
                            {
                                return TestSuiteResult.Error("No results", TimeSpan.Zero);
                            }

                            runner.Unload();
                            var parsedResults = CreateResult(result);
                            return parsedResults;
                        }
                        catch (Exception ex)
                        {
                            TryStopRunner(runner);
                            return TestSuiteResult.Error($"Unkown nunit error: {ex.Message}", TimeSpan.Zero);
                        }
                    }
                }
            });
        }

        private TestSuiteResult CreateResult(XmlNode result)
        {
            var nUnitTestCaseResultMaker = new NUnitTestCaseResultMaker();
            if (result.Name != "test-run")
            {
                return TestSuiteResult.Error("Expected <test-run> as top-level element but was <" + result.Name + ">", TimeSpan.Zero);
            }

            var name = result.GetAttribute("name");
            var testCaseResults = nUnitTestCaseResultMaker.CreateTestCaseResult(result);
            var duration = double.Parse(result.GetAttribute("duration"), CultureInfo.InvariantCulture);

            return new TestSuiteResult
            {
                Name = name,
                TestResults = testCaseResults,
                InnerXml = result.InnerXml,
                ExecutionTime = TimeSpan.FromSeconds(duration),
                IsSuccess = testCaseResults.All(t => t.IsSuccess)
            };
        }

        private void TryStopRunner(global::NUnit.Engine.ITestRunner testRunner)
        {
            // Really hacky but sometimes it fail..
            for (int n = 0; n < 3; n++)
            {
                try
                {
                    testRunner.StopRun(true);
                    testRunner.Unload();
                }
                catch (Exception)
                {
                    Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
