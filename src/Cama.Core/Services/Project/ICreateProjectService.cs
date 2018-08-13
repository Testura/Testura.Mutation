using Cama.Core.Models.Mutation;

namespace Cama.Core.Services.Project
{
    public interface ICreateProjectService
    {
        void CreateProject(string savePath, CamaConfig config);
    }
}