using System;
using System.IO;
using log4net;

namespace Testura.Mutation.Core.Util.FileSystem
{
    public class DirectoryHandler : IDirectoryHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DirectoryHandler));

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
                Log.Error($"Failed to delete baseline directory: {ex.Message}");
            }
        }
    }
}
