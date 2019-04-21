using Cama.Console.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Cama.Console.CommandConfigurations
{
    public class MutateGitConfiguration
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Description = "An example command from the neat .NET Core Starter";
            app.HelpOption("--help|-h|-?");

            var configPath = app.Option("-cp|--configPath", "Required. Path to cama config", CommandOptionType.SingleValue).IsRequired();
            var outputPath = app.Option("-op|--outputPath", "Required. Path to output directory", CommandOptionType.SingleValue).IsRequired();

            app.OnExecute(() =>
            {
                var command = new MutateLocalCommand(configPath.Value(), outputPath.Value());
                return command.RunAsync();
            });
        }
    }
}
