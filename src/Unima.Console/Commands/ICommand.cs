using System.Threading.Tasks;

namespace Unima.Console.Commands
{
    public interface ICommand
    {
        Task<int> RunAsync();
    }
}
