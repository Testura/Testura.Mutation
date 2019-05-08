using System.Threading.Tasks;
using MediatR;
using Microsoft.Practices.Unity;

namespace Cama.Console.Commands
{
    public class MutateGitCommand : ICommand
    {
        private readonly string _repositoryUrl;
        private readonly string _branch;
        private readonly string _username;
        private readonly string _password;
        private readonly string _outputPath;

        public MutateGitCommand(string repositoryUrl, string branch, string username, string password, string outputPath)
        {
            _repositoryUrl = repositoryUrl;
            _branch = branch;
            _username = username;
            _password = password;
            _outputPath = outputPath;
        }

        public async Task<int> RunAsync()
        {
            var mediator = Bootstrapper.GetContainer().Resolve<IMediator>();

            /*
            MSBuildLocator.RegisterDefaults();
            var mutationRunner = Bootstrapper.GetContainer().Resolve<ConsoleMutationExecutor>();
            var success = await mutationRunner.ExecuteMutationRunner(_configPath, _outputPath);

            if (!success)
            {
                return -1;
            }

            return 0;
            */

            return await Task.FromResult(1);
        }
    }
}
