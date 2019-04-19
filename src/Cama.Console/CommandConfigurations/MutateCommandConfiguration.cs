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

            var configPath = command.Option("-cp|--configPath", "Required. Path to cama config", CommandOptionType.SingleValue).IsRequired();
            var outputPath = command.Option("-op|--outputPath", "Required. Path to output directory", CommandOptionType.SingleValue).IsRequired();

            command.OnExecute(() =>
            {
                options.Command = new MutateCommand(configPath.Value(), outputPath.Value(), options);

                return 0;
            });
        }
    }
}
