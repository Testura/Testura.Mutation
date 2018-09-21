using Cama.Core.Config;

namespace Cama.Core.Services.Project
{
    public interface ICreateCamaProjectService
    {
        void CreateProject(string savePath, CamaFileConfig config);
    }
}