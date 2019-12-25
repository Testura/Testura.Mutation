using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Medallion.Shell;
using Medallion.Shell.Streams;
using Newtonsoft.Json;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;

namespace Testura.Mutation.Infrastructure
{
    public class TestRunnerConsoleClient : ITestRunnerClient

    {
        public Task<TestSuiteResult> RunTestsAsync(string runner, string dllPath, string dotNetPath, TimeSpan maxTime)
        {
            return Task.Run(() =>
            {
                var allArguments = new List<string>
                {
                    runner,
                    dllPath,
                    maxTime.ToString(),
                    dotNetPath
                };

                try
                {
                    using (var command = Command.Run(
                        "Testura.Mutation.TestRunner.Console.exe",
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
                        var error = string.Empty;
                        var success = ReadToEnd(command.StandardOutput, maxTime, out var output) && ReadToEnd(command.StandardError, maxTime, out error);

                        if (!success)
                        {
                            command.Kill();
                            throw new MutationDocumentException("We have a problem reading from stream. Killing this mutation");
                        }

                        try
                        {
                            if (!command.Result.Success)
                            {
                                LogTo.Info($"Message from test client - {output}.");
                                LogTo.Info($"Error from test client - {error}.");

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
                            LogTo.Info("Test client timed out. Infinite loop?");
                            return TestSuiteResult.Error("TIMEOUT", maxTime);
                        }

                        return JsonConvert.DeserializeObject<TestSuiteResult>(output);
                    }
                }
                catch (Win32Exception ex)
                {
                    LogTo.ErrorException("Unknown exception from test client process", ex);
                    throw;
                }
            });
        }

        private bool ReadToEnd(ProcessStreamReader processStream, TimeSpan maxTime, out string message)
        {
            var readStreamTask = Task.Run(() => processStream.ReadToEnd());
            // We also have a max time in the test runner so add a bit of extra here 
            // just in case so we don't fail it to early.
            var successful = readStreamTask.Wait(maxTime.Add(TimeSpan.FromSeconds(30))); 

            message = successful ? readStreamTask.Result : "Stuck when reading from stream!";
            return successful;
        }
    }
}
