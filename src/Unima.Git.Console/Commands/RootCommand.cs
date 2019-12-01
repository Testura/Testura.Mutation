using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Unima.Git.Commands
{
    public class RootCommand : ICommand
    {
        private CommandLineApplication _app;

        public RootCommand(CommandLineApplication app)
        {
            _app = app;
        }

        public Task<int> RunAsync()
        {
            _app.ShowHelp();
            return Task.FromResult(-1);
        }
    }
}
