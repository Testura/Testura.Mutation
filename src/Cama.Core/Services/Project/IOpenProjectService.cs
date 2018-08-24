using System.Threading.Tasks;
using Cama.Core.Models.Project;

namespace Cama.Core.Services.Project
{
    public interface IOpenProjectService
    {
        Task<CamaRunConfig> OpenProjectAsync(string path);
    }
}