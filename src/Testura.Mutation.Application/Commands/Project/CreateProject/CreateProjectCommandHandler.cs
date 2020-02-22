using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR;
using Newtonsoft.Json;

namespace Testura.Mutation.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, bool>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CreateProjectCommandHandler));

        public Task<bool> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
        {
            Log.Info("Creating project file");
            File.WriteAllText(command.SavePath, JsonConvert.SerializeObject(command.Config));

            return Task.FromResult(true);
        }
    }
}
