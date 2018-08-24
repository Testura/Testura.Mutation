using Cama.Core.Models.Project;

namespace Cama.Core.Services.Project
{
    public interface ICreateProjectService
    {
        void CreateProject(string savePath, CamaLocalConfig config);
    }
}