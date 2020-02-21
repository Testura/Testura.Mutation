using System;
using System.IO;
using Anotar.Log4Net;

namespace Testura.Mutation.Core.Util.FileSystem
{
    public class DirectoryHandler : IDirectoryHandler
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                LogTo.Error($"Failed to delete baseline directory: {ex.Message}");
            }
        }
    }
}
