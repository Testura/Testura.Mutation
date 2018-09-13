using System.IO;
using System.Linq;
using Anotar.Log4Net;

namespace Cama.Core.Services
{
    public class DependencyFilesHandler
    {
        public void CopyDependencies(string path, string targetPath)
        {
            var files = 0;

            Directory
                .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .AsParallel()
                .ForAll(from =>
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
                });

            if (files == 0)
            {
                LogTo.Warn($"Did not find any files to copy at \"{path}\". Make sure that you built the solution before running mutation tests.");
            }
        }
    }
}
