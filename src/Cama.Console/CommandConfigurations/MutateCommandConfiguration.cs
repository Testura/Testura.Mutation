using Cama.Console.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Cama.Console.CommandConfigurations
{
    public class MutateCommandConfiguration
    {
        public static void Configure(CommandLineApplication command, CommandLineOptions options)
        {
            command.Description = "An example command from the neat .NET Core Starter";
            command.HelpOption("--help|-h|-?");

            var configPathArgument = command.Argument("configPath", "Path to cama config");
            var outputPathArgument = command.Argument("outputPath", "Path to output directory");

            command.OnExecute(() =>
            {
                options.Command = new MutateCommand(configPathArgument.Value, outputPathArgument.Value, options);

                return 0;
            });
        }
    }
}
