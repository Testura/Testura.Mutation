using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Execution.Result;
using Cama.Core.Execution.Runners;
using Xunit.Runners;

namespace Cama.TestRunner.XUnit
{
    public class XUnitTestRunner : ITestRunner
    {
        private double _executionTime;
        private ManualResetEvent _finished;
        private IList<TestResult> _results;

        public async Task<TestSuiteResult> RunTestsAsync(string dllPath, TimeSpan maxTime)
        {
            _executionTime = 0;
            _finished = new ManualResetEvent(false);
            _results = new List<TestResult>();

            using (var runner = AssemblyRunner.WithAppDomain(dllPath, shadowCopy: false))
            {
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestPassed = OnTestPassed;
                var resultTask = Task.Run(() => RunTests(runner));

                var finishedTask = await Task.WhenAny(resultTask, Task.Delay(maxTime));

                if (finishedTask != resultTask)
                {
                    LogTo.Info("Test canceled. The mutation probably created an infinite loop.");
                    runner.Cancel();
                    return new TestSuiteResult("TIMEOUT", new List<TestResult>(), "NULL", maxTime);
                }

                _finished.WaitOne();
                _finished.Dispose();

                return new TestSuiteResult(Path.GetFileName(dllPath), _results, string.Empty, TimeSpan.FromSeconds((int)_executionTime));
            }
        }

        private void RunTests(AssemblyRunner runner)
        {
            try
            {
               runner.Start();
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Failed to unload test runner", ex);
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
