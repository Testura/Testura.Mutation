using System;
using System.IO;
using System.Linq;
using Anotar.Log4Net;

namespace Testura.Mutation.Core.Execution
{
    public class TestRunnerDependencyFilesHandler
    {
        public void CopyDependencies(string path, string targetPath)
        {
            var files = 0;

            try
            {
                Directory
                    .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                    .AsParallel()
                    .ForAll(from =>
                    {
                        try
                        {
                            var to = from.Replace(path, targetPath);

                            // Create directories if need
                            var toSubFolder = Path.GetDirectoryName(to);
                            if (!string.IsNullOrWhiteSpace(toSubFolder))
                            {
                                Directory.CreateDirectory(toSubFolder);
                            }

                            files++;
                            File.Copy(from, to, true);
                        }
                        catch (Exception ex)
                        {
                            var o = ex;
                        }
                    });
            }
            catch (Exception ex)
            {
                var o = ex;
            }

            if (files == 0)
            {
                LogTo.Warn($"Did not find any files to copy at \"{path}\". Make sure that you built the solution before running mutation tests.");
            }
        }
    }
}
