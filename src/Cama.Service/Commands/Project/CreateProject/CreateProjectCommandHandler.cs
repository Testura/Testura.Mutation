using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Newtonsoft.Json;

namespace Cama.Service.Commands.Project.CreateProject
{
    public class CreateProjectCommandHandler : ValidateResponseRequestHandler<CreateProjectCommand, bool>
    {
        public override Task<bool> OnHandle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(command.SavePath, JsonConvert.SerializeObject(command.Config));

            return Task.FromResult(true);
        }
    }
}
