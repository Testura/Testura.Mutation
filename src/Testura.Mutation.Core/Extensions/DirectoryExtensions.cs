using System;
using System.IO.Abstractions;
using log4net;

namespace Testura.Mutation.Core.Extensions
{
    public static class DirectoryExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DirectoryExtensions));

        public static void DeleteDirectoryAndCheckForException(this IDirectory directory, string path)
        {
            try
            {
                if (directory.Exists(path))
                {
                    directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to delete baseline directory: {ex.Message}");
            }
        }
    }
}
