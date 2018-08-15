using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TestRunner : ITestRunner
    {
        public TestSuiteResult RunTests(string dllPath, IList<string> testNames)
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
                    var filter = CreateFilter(testNames, engine.Services.GetService<ITestFilterService>().GetTestFilterBuilder());
                    var result = runner.Run(new TestEventDispatcher(), filter);
                    try
                    {
                        if (result.InnerText.Contains("System.IO.FileLoadException : Could not load file or assembly "))
                        {
                        }

                        runner.Unload();
                        return CreateResult(result);
                    }
                    catch (Exception ex)
                    {
                        LogTo.ErrorException("Failed to unload test runner", ex);
                    }

                    return new TestSuiteResult("ERROR", new List<TestResult>(), "NULL");
                }
            }
        }

        public TestFilter CreateFilter(IList<string> testNames, ITestFilterBuilder builder)
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

        private static TestSuiteResult CreateResult(XmlNode result)
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
