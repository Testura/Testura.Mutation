using McMaster.Extensions.CommandLineUtils;
using Unima.Git.Commands;

namespace Unima.Git.CommandConfigurations
{
    public class MutateGitConfiguration
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Description = "An example command from the neat .NET Core Starter";
            app.HelpOption("--help|-h|-?");

            var repositoryUrl = app.Option("-ru|--repositoryUrl", "Required. Path to unima config", CommandOptionType.SingleValue).IsRequired();
            var branch = app.Option("-b|--branch", "Required. Path to unima config", CommandOptionType.SingleValue).IsRequired();
            var outputPath = app.Option("-op|--outputPath", "Required. Path to output directory", CommandOptionType.SingleValue).IsRequired();

            var username = app.Option("-u|--username", "Required. Path to unima config", CommandOptionType.SingleValue);
            var password = app.Option("-p|--password", "Required. Path to unima config", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                var command = new MutateGitCommand(repositoryUrl.Value(), branch.Value(), username.Value(), password.Value(), outputPath.Value());
                return command.RunAsync();
            });
        }
    }
}
