using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;

namespace Cama.Console.Commands
{
    public class MutateCommand : ICommand
    {
        private readonly string _configPath;
        private readonly string _outputPath;
        private CommandLineOptions _options;

        public MutateCommand(string configPath, string outputPath, CommandLineOptions options)
        {
            _configPath = configPath;
            _outputPath = outputPath;
            _options = options;
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
