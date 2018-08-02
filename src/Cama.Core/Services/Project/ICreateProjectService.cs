using Cama.Core.Models.Mutation;

namespace Cama.Core.Services.Project
{
    public interface ICreateProjectService
    {
        void CreateProject(CamaConfig config);
    }
}