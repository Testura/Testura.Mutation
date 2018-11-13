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
        private ManualResetEvent _finished;
        private IList<TestResult> _results;

        public async Task<TestSuiteResult> RunTestsAsync(string dllPath, TimeSpan maxTime)
        {
            _finished = new ManualResetEvent(false);
            _results = new List<TestResult>();

            using (var runner = AssemblyRunner.WithAppDomain(dllPath, shadowCopy: false))
            {
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestPassed = OnTestPassed;

                var maxTimeTask = Task.Run(async () => await Task.Delay(maxTime));
                var resultTask = Task.Run(() => RunTests(runner));

                var finishedTask = await Task.WhenAny(maxTimeTask, resultTask);

                if (finishedTask == maxTimeTask)
                {
                    LogTo.Info("Test canceled. The mutation probably created an infinite loop.");
                    runner.Cancel();
                    return new TestSuiteResult("TIMEOUT", new List<TestResult>(), "NULL");
                }

                _finished.WaitOne();
                _finished.Dispose();

                return new TestSuiteResult(Path.GetFileName(dllPath), _results, string.Empty);
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
            _finished.Set();
        }
    }
}
