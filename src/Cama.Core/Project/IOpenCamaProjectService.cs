using System.Threading.Tasks;
using Cama.Core.Config;

namespace Cama.Core.Services.Project
{
    public interface IOpenCamaProjectService
    {
        Task<CamaConfig> OpenProjectAsync(string path);
    }
}