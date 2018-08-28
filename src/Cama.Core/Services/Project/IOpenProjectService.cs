using System.Threading.Tasks;
using Cama.Core.Models.Project;

namespace Cama.Core.Services.Project
{
    public interface IOpenProjectService
    {
        Task<CamaConfig> OpenProjectAsync(string path);
    }
}