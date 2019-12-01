using System;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using Unima.Git.CommandConfigurations;
using Unima.Git.Commands;

namespace Unima.Git
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Unima.Git",
                FullName = "C# Git cloner"
            };

            app.HelpOption("-?|-h|--help");

            app.Command("git", a => MutateGitConfiguration.Configure(a));

            app.OnExecute(() => new RootCommand(app).RunAsync().Wait());

            var result = app.Execute(args);
            Environment.Exit(result);
        }
    }
}
