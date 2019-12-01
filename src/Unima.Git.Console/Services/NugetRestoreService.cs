using System;
using System.Threading.Tasks;
using Medallion.Shell;
using Medallion.Shell.Streams;

namespace Unima.Git.Services
{
    public class NugetRestoreService
    {
        public void PerformNugetRestore(string slnPath)
        {
            using (var command = Command.Run(
                "nuget.exe",
                new[] { "restore", slnPath },
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
                    o.DisposeOnExit();
                }))
            {
                var success = ReadToEnd(command.StandardError, out var error);

                if (!success)
                {
                    command.Kill();
                }

                if (!success || (!command.Result.Success && !error.ToLower().Contains("test run failed")))
                {
                }
            }
        }

        private bool ReadToEnd(ProcessStreamReader processStream, out string message)
        {
            var readStreamTask = Task.Run(() => processStream.ReadToEnd());
            var successful = readStreamTask.Wait(TimeSpan.FromSeconds(30));

            message = successful ? readStreamTask.Result : "Error reading from stream";
            return successful;
        }
    }
}
