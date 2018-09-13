using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Anotar.Log4Net;
using Cama.Core.Models;
using Cama.Core.TestRunner.Result.Maker;
using NUnit;
using NUnit.Engine;
using NUnit.Engine.Runners;
using NUnit.Engine.Services;
using TestSuiteResult = Cama.Core.Models.TestSuiteResult;

namespace Cama.Core.TestRunner
{
    public class NUnitTestRunner : ITestRunner
    {
        public async Task<TestSuiteResult> RunTestsAsync(string dllPath, IList<string> testNames, TimeSpan maxTime)
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

                using (NUnit.Engine.ITestRunner runner = engine.GetRunner(package))
                {
                    try
                    {
                        var filter = CreateFilter(testNames, engine.Services.GetService<ITestFilterService>().GetTestFilterBuilder());

                        var maxTimeTask = Task.Run(async () => await Task.Delay(maxTime));
                        var resultTask = Task.Run(() => RunTests(runner, filter));

                        var finishedTask = await Task.WhenAny(maxTimeTask, resultTask);

                        if (finishedTask == maxTimeTask)
                        {
                            LogTo.Info("Test canceled. The mutation probably created an infinite loop.");
                            runner.StopRun(true);
                            return new TestSuiteResult("TIMEOUT", new List<TestResult>(), "NULL");
                        }

                        var result = resultTask.Result;

                        if (result == null)
                        {
                            return new TestSuiteResult("ERROR", new List<TestResult>(), "NULL");
                        }

                        if (result.InnerText.Contains("System.IO.FileLoadException : Could not load file or assembly "))
                        {
                            LogTo.Error(
                                $"Problem with loading file or assembly formation. Make sure that we have all required dependencies: \"{result.InnerText}\"");
                        }

                        runner.Unload();
                        var parsedResults = CreateResult(result);

                        if (!parsedResults.TestResults.Any())
                        {
                            LogTo.Info("Didn't run any test. Please check the inner xml: ");
                            LogTo.Info(result.InnerXml);
                            LogTo.Info(result.InnerText);
                        }

                        return parsedResults;
                    }
                    catch (Exception ex)
                    {
                        LogTo.ErrorException("Failed to unload test runner", ex);
                    }
                }

                return new TestSuiteResult("ERROR", new List<TestResult>(), "NULL");
            }
        }

        private XmlNode RunTests(NUnit.Engine.ITestRunner runner, TestFilter filter)
        {
            try
            {
                return runner.Run(new TestEventDispatcher(), filter);
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Failed to unload test runner", ex);
                return null;
            }
        }

        private TestFilter CreateFilter(IList<string> testNames, ITestFilterBuilder builder)
        {
            if (!testNames.Any())
            {
                return TestFilter.Empty;
            }

            foreach (var testName in testNames)
            {
                builder.AddTest(testName);
            }

            return builder.GetFilter();
        }

        private TestSuiteResult CreateResult(XmlNode result)
        {
            var nUnitTestCaseResultMaker = new NUnitTestCaseResultMaker();
            if (result.Name != "test-run")
            {
                throw new InvalidOperationException("Expected <test-run> as top-level element but was <" + result.Name + ">");
            }

            var name = result.GetAttribute("name");
            var testCaseResults = nUnitTestCaseResultMaker.CreateTestCaseResult(result);
            return new TestSuiteResult(name, testCaseResults, result.InnerXml);
        }
    }
}
