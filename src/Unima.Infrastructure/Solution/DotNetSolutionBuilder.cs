using System;
using Anotar.Log4Net;
using Medallion.Shell;
using Unima.Core.Solution;
using Unima.Infrastructure.Stream;

namespace Unima.Infrastructure.Solution
{
    public class DotNetSolutionBuilder : StreamReaderBase, ISolutionBuilder
    {
        public void BuildSolution(string solutionPath)
        {
            LogTo.Info("Building solution..");

            using (var command = Command.Run(
                "dotnet.exe",
                new[] { "build", solutionPath },
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
                    LogTo.Warn("..failed to build solution");
                    LogTo.Warn(error);
                    throw new Exception(error);
                }
                
                LogTo.Info("..build was successful");
            }
        }
    }
}
