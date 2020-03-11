using System;
using System.IO;
using log4net;
using Medallion.Shell;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Infrastructure.Stream;

namespace Testura.Mutation.Infrastructure.Git
{
    public class GitDIff : StreamReaderBase, IGitDiff
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GitDIff));

        public string GetDiff(string path, string branch)
        {
            Log.Info("Getting git diff..");

            using (var command = Command.Run(
                "git.exe",
                new[] { "--git-dir", $"{Path.Combine(path, ".git")}", "diff", "HEAD^","HEAD", "-U0" },
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
                ReadToEnd(command.StandardOutput, out var diff);

                if (!success)
                {
                    command.Kill();
                    Log.Warn("..failed to get git diff");
                    Log.Warn(error);
                    throw new Exception(error);
                }

                Log.Info(".. git diff was successful. Creating filer items");

                return diff;
            }
        }
    }
}
