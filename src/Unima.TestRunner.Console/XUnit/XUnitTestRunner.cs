using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unima.Core.Execution.Result;
using Unima.Core.Execution.Runners;
using Xunit.Runners;

namespace Unima.TestRunner.Console.XUnit
{
    public class XUnitTestRunner : ITestRunner
    {
        private double _executionTime;
        private ManualResetEvent _finished;
        private IList<TestResult> _results;

        public async Task<TestSuiteResult> RunTestsAsync(string dllPath)
        {
            _executionTime = 0;
            _finished = new ManualResetEvent(false);
            _results = new List<TestResult>();

            using (var runner = AssemblyRunner.WithAppDomain(dllPath, shadowCopy: false))
            {
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestPassed = OnTestPassed;
                await Task.Run(() => RunTests(runner));

                _finished.WaitOne();
                _finished.Dispose();

                return new TestSuiteResult
                {
                    Name = Path.GetFileName(dllPath),
                    TestResults = _results,
                    ExecutionTime = TimeSpan.FromSeconds((int)_executionTime),
                    IsSuccess = _results.All(r => r.IsSuccess)
                };
            }
        }

        private void RunTests(AssemblyRunner runner)
        {
            try
            {
               runner.Start();
            }
            catch
            {
                /* LogTo.ErrorException("Failed to unload test runner", ex); */
                _finished.Set();
            }
        }

        private void OnTestPassed(TestPassedInfo info)
        {
            _results.Add(new TestResult
            {
                FullName = $"{info.TypeName}.{info.MethodName}",
                InnerText = string.Empty,
                IsSuccess = true,
                Name = info.MethodName
            });
        }

        private void OnTestFailed(TestFailedInfo info)
        {
            _results.Add(new TestResult
            {
                FullName = $"{info.TypeName}.{info.MethodName}",
                InnerText = info.ExceptionMessage,
                IsSuccess = false,
                Name = info.MethodName
            });
        }

        private void OnExecutionComplete(ExecutionCompleteInfo info)
        {
            _executionTime = (double)info.ExecutionTime;
            _finished.Set();
        }
    }
}
