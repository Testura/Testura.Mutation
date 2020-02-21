namespace Testura.Mutation.Core.Util.FileSystem
{
    public interface IDirectoryHandler
    {
        void CreateDirectory(string path);

        void DeleteDirectory(string path);
    }
}
