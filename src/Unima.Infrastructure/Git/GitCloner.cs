using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Medallion.Shell;
using Medallion.Shell.Streams;
using Unima.Core.Git;

namespace Unima.Infrastructure.Git
{
    public class GitCloner : IGitCloner
    {
        public void CloneProject(string repositoryUrl, string branch, string username, string password, string outputPath)
        {
            var arguments = new List<string> { "git", "-ru", repositoryUrl, "-b", branch, "-op", outputPath };

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                arguments.AddRange(new[]{ "-u", username, "-p", password});
            }

            using (var command = Command.Run(
                "Unima.Git.exe",
                arguments.ToArray(),
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
