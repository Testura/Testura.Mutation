using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Medallion.Shell;
using Medallion.Shell.Streams;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Infrastructure.Stream;

namespace Testura.Mutation.Infrastructure.Git
{
    public class GitDIff : StreamReaderBase, IGitDiff
    {
        public string GetDiff(string path, string branch)
        {
            LogTo.Info("Getting git diff..");

            using (var command = Command.Run(
                "git.exe",
                new[] { "--git-dir", $"{Path.Combine(path, ".git")}", "diff", "HEAD^" ,"HEAD", "-U0" },
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
                    LogTo.Warn("..failed to get git diff");
                    LogTo.Warn(error);
                    throw new Exception(error);
                }

                LogTo.Info(".. git diff was successful. Creating filer items");

                return diff;
            }
        }
    }
}
