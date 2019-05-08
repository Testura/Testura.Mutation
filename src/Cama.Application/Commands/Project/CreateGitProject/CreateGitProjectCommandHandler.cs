using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Cama.Application.Commands.Project.CreateGitProject
{
    public class CreateGitProjectCommandHandler : IRequestHandler<CreateGitProjectCommand, bool>
    {
        public Task<bool> Handle(CreateGitProjectCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
