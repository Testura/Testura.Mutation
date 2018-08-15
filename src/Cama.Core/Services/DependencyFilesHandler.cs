using System.IO;
using System.Linq;

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

                    File.Copy(from, to, true);
                });
        }
    }
}
