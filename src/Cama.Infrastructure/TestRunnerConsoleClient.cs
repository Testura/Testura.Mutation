using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Execution.Result;
using Cama.Core.Execution.Runners;
using Medallion.Shell;
using Newtonsoft.Json;

namespace Cama.Infrastructure
{
    public class TestRunnerConsoleClient : ITestRunnerClient

    {
        public Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, TimeSpan maxTime)
        {
            return Task.Run(() =>
            {
                var allArguments = new List<string>
                {
                    runner,
                    dllPath
                };

                try
                {
                    using (var command = Command.Run(
                        "Cama.TestRunner.Console.exe",
                        allArguments,
                        o =>
                        {
                            o.StartInfo(si =>
                            {
                                si.CreateNoWindow = true;
                                si.UseShellExecute = false;
                                si.RedirectStandardError = true;
                                si.RedirectStandardInput = true;
                                si.RedirectStandardOutput = true;
                            });
                            o.Timeout(maxTime);
                            o.DisposeOnExit();
                        }))
                    {
                        var output = command.StandardOutput.ReadToEnd();
                        var error = command.StandardError.ReadToEnd();

                        try
                        {
                            if (!command.Result.Success)
                            {
                                LogTo.Info($"Error from test client - {error}");
                                return new TestSuiteResult
                                {
                                    IsSuccess = false,
                                    Name = $"ERROR - {error}",
                                    ExecutionTime = TimeSpan.Zero,
                                    TestResults = new List<TestResult>()
                                };
                            }
                        }
                        catch (TimeoutException)
                        {
                            LogTo.Info("Test client timed out. Infinit loop?");
                            return TestSuiteResult.Error("TIMEOUT", maxTime);
                        }


                        return JsonConvert.DeserializeObject<TestSuiteResult>(output);

                    }
                }
                catch (Win32Exception ex)
                {
                    LogTo.ErrorException("Unknown expcetion from test client process", ex);
                    throw;
                }
            });
        }
    }
}
