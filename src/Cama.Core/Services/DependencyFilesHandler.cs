using System.IO;
using System.Linq;
using Anotar.Log4Net;

namespace Cama.Core.Services
{
    public class DependencyFilesHandler
    {
        public void CopyDependencies(string path, string targetPath)
        {
            LogTo.Info("Copying all dependecies..");
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

            LogTo.Info($"..copying done ({files} files)");
        }
    }
}
