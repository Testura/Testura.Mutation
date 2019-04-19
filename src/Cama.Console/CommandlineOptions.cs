using Cama.Console.CommandConfigurations;
using Cama.Console.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Cama.Console
{
    public class CommandLineOptions
    {
        public ICommand Command { get; set; }

        public static CommandLineOptions Parse(string[] args)
        {
            var options = new CommandLineOptions();

            var app = new CommandLineApplication
            {
                Name = "Cama.Console",
                FullName = "C# mutation testing"
            };

            app.HelpOption("-?|-h|--help");

            MutateCommandConfiguration.Configure(app, options);

            var result = app.Execute(args);

            if (result != 0)
            {
                return null;
            }

            return options;
        }
    }
}
