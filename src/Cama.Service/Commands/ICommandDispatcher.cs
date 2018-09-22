using System.Threading.Tasks;
using MediatR;

namespace Cama.Service.Commands
{
    public interface ICommandDispatcher
    {
        Task<T> ExecuteCommandAsync<T>(IRequest<T> command);
    }
}
