using System;
using System.Collections.Generic;
using System.Xml;
using NUnit;
using NUnit.Engine;
using NUnit.Engine.Runners;
using NUnit.Engine.Services;
using Testura.Module.TestRunner.Result;
using Testura.Module.TestRunner.Result.Maker;


namespace Testura.Module.TestRunner
{
    public class TestRunner : ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <param name="testCaseRunInformations">Information about tests cases and data sets to run</param>
        /// <returns>Results from the nunit remote tester</returns>
        public NUnitTestSuiteResult RunTests(string dllPath, IList<string> testNames)
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
                //engine.Services.Add(new RecentFilesService());
                //engine.Services.Add(new DefaultTestRunnerFactory());
                //engine.Services.Add(new TestAgency());
                engine.Services.Add(new ResultService());
                engine.Services.ServiceManager.StartServices();

                using (NUnit.Engine.ITestRunner runner = engine.GetRunner(package))
                {
                    var c = runner.CountTestCases(TestFilter.Empty);

                    var result = runner.Run(new TestEventDispatcher(), CreateFilter(testNames, engine.Services.GetService<ITestFilterService>().GetTestFilterBuilder()));
                    runner.Unload();
                    return CreateResult(result);
                }
            }

            //using (ITestEngine engine = TestEngineActivator.CreateInstance(unused: true))
            //{
            //    var services = engine.Services;

            //    using (NUnit.Engine.ITestRunner runner = engine.GetRunner(package))
            //    {
            //        var o = runner.Explore(TestFilter.Empty);
            //        var result = runner.Run(new TestEventDispatcher(), CreateFilter(testNames, engine.Services.GetService<ITestFilterService>().GetTestFilterBuilder()));
            //        runner.Unload();
            //        return CreateResult(result);
            //    }
            //}


            //CoreExtensions.Host.InitializeService();
            //var testPackage = new TestPackage(dllPath);
            //var remoteTestRunner = new RemoteTestRunner();
            //remoteTestRunner.Load(testPackage);
            //var builder = new TestSuiteBuilder();
            //var testSuit = builder.Build(testPackage).Tests[0] as TestSuite;
            //var filter = GetFilter(testSuit, testCaseRunInformations);
            //TestResult result = testSuit. Run(new NullListener(), filter);
            //remoteTestRunner.Unload();
            //CoreExtensions.Host.UnloadService();
            //return result;
        }

        private static NUnitTestSuiteResult CreateResult(XmlNode result)
        {
            var nUnitTestCaseResultMaker = new NUnitTestCaseResultMaker();
            if (result.Name != "test-run")
                throw new InvalidOperationException("Expected <test-run> as top-level element but was <" + result.Name + ">");
            var name = result.GetAttribute("name");
            var testCaseResults = nUnitTestCaseResultMaker.CreateTestCaseResult(result);
            return new NUnitTestSuiteResult(name, testCaseResults, result.InnerXml);
        }


        public TestFilter CreateFilter(IList<string> testNames, ITestFilterBuilder builder)
        {
            foreach (var testName in testNames)
            {
                builder.AddTest(testName);
            }

            return builder.GetFilter();
        }
   




        /// <summary>
        /// Get our test run filter
        /// </summary>
        /// <param name="testSuit"></param>
        /// <param name="methodsToRun"></param>
        /// <returns></returns>
        //private ITestFilter GetFilter(NUnit.Core.TestSuite testSuit, IList<TestCaseRunInformation> testCaseRunInformations)
        //{
        //    //if (testCaseRunInformations == null || testCaseRunInformations.Count == 0)
        //    //    return TestFilter.Empty;
        //    //var filter = new NameFilter();
        //    //foreach (var testFixture in testSuit.Tests.Cast<TestFixture>())
        //    //{
        //    //    foreach (var testMethod in testFixture.Tests.Cast<NUnitTestMethod>())
        //    //    {
        //    //        if(testCaseRunInformations.Any(t => t.TestCaseName == testFixture.TestName.Name && t.DataSets.Select(d => d.Replace(" ", "_")).Contains(testMethod.TestName.Name)))
        //    //            filter.Add(testMethod.TestName);
        //    //    }
        //    //}
        //    //return filter;
        //}
    }
}
