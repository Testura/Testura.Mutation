using System.Threading.Tasks;

namespace Unima.Git.Commands
{
    public interface ICommand
    {
        Task<int> RunAsync();
    }
}
