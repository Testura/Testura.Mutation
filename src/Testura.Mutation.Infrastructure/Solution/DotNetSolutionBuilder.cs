using System;
using Anotar.Log4Net;
using Medallion.Shell;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Infrastructure.Stream;

namespace Testura.Mutation.Infrastructure.Solution
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
                var successError = ReadToEnd(command.StandardError, out var error);
                ReadToEnd(command.StandardOutput, out var standardOutput);

                if (!successError || standardOutput.Contains("Build FAILED"))
                {
                    command.Kill();
                    LogTo.Error("..failed to build solution");
                    LogTo.Error(error);
                    LogTo.Error(standardOutput);
                    throw new Exception(error);
                }

                LogTo.Info("..build was successful");
            }
        }
    }
}
