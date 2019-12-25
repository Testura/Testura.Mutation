using System.Threading.Tasks;

namespace Testura.Mutation.Console.Commands
{
    public interface ICommand
    {
        Task<int> RunAsync();
    }
}
