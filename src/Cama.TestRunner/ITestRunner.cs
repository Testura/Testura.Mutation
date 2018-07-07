using System.Collections.Generic;
using Testura.Module.TestRunner.Result;

namespace Testura.Module.TestRunner
{
    public interface ITestRunner
    {
        /// <summary>
        /// Run unit test with nunit remote tester
        /// </summary>
        /// <param name="dllPath">Path to the dll that contains all our unit tests</param>
        /// <returns>Results from the nunit remote tester</returns>
        NUnitTestSuiteResult RunTests(string dllPath, IList<string> testNames);
    }
}