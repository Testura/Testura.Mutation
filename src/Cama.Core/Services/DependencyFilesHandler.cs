using System.IO;
using System.Linq;
using Anotar.Log4Net;

namespace Cama.Core.Services
{
    public class DependencyFilesHandler
    {
        public void CopyDependencies(string path, string targetPath)
        {
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

                    LogTo.Info($"Copying from \"{from}\" to \"{to}\"");
                    File.Copy(from, to, true);
                });
        }
    }
}
