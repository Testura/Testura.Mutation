using Cama.Core.Models.Mutation;

namespace Cama.Core.Services.Project
{
    public interface IOpenProjectService
    {
        CamaConfig OpenProject(string path);
    }
}