using System.Threading.Tasks;
using MediatR;

namespace Cama.Service.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IMediator _mediator;

        public CommandDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<T> ExecuteCommandAsync<T>(IRequest<T> command)
        {
            return await _mediator.Send(command);
        }
    }
}
