using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Testura.Mutation.Console.Commands
{
    public class MutateLocalCommand : ICommand
    {
        private readonly string _configPath;
        private readonly string _outputPath;

        public MutateLocalCommand(string configPath, string outputPath)
        {
            _configPath = configPath;
            _outputPath = outputPath;
        }

        public async Task<int> RunAsync()
        {
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
