using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;

namespace Cama.Console.Commands
{
    public class MutateGitCommand : ICommand
    {
        private readonly string _configPath;
        private readonly string _outputPath;

        public MutateGitCommand(string configPath, string outputPath)
        {
            _configPath = configPath;
            _outputPath = outputPath;
        }

        public async Task<int> RunAsync()
        {
            MSBuildLocator.RegisterDefaults();
            var mutationRunner = Bootstrapper.GetContainer().Resolve<ConsoleMutationExecutor>();
            var success = await mutationRunner.ExecuteMutationRunner(_configPath, _outputPath);

            if (!success)
            {
                return -1;
            }

            return 0;
        }
    }
}
