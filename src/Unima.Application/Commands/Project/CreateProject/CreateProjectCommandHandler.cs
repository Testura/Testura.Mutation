using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using MediatR;
using Newtonsoft.Json;

namespace Unima.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, bool>
    {
        public Task<bool> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(command.SavePath, JsonConvert.SerializeObject(command.Config));

            return Task.FromResult(true);
        }
    }
}
