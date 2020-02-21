using System;
using log4net;
using Medallion.Shell;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Infrastructure.Stream;

namespace Testura.Mutation.Infrastructure.Solution
{
    public class DotNetSolutionBuilder : StreamReaderBase, ISolutionBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DotNetSolutionBuilder));

        public void BuildSolution(string solutionPath)
        {
            Log.Info("Building solution..");

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
                    Log.Error("..failed to build solution");
                    Log.Error(error);
                    Log.Error(standardOutput);
                    throw new Exception(error);
                }

                Log.Info("..build was successful");
            }
        }
    }
}
