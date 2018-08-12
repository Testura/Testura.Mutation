using System.IO;
using System.Linq;

namespace Cama.Core.Services
{
    public class DependencyFilesHandler
    {
        public void CopyDependencies(string path, string targetPath)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files.Where(f => f.EndsWith(".dll") && !f.Contains("Testura.Code.dll")))
            {
                File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), true);
            }
        }
    }
}
