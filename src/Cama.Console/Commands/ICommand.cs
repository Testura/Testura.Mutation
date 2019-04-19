using System.Threading.Tasks;

namespace Cama.Console.Commands
{
    public interface ICommand
    {
        Task<int> RunAsync();
    }
}
