using System;
using Cama.Console.CommandConfigurations;
using Cama.Console.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Cama.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine("________/\\\\\\\\\\\\\\\\\\_____/\\\\\\\\\\\\\\\\\\_____/\\\\\\\\____________/\\\\\\\\_____/\\\\\\\\\\\\\\\\\\____        ");
            System.Console.WriteLine(" _____/\\\\\\////////____/\\\\\\\\\\\\\\\\\\\\\\\\\\__\\/\\\\\\\\\\\\________/\\\\\\\\\\\\___/\\\\\\\\\\\\\\\\\\\\\\\\\\__       ");
            System.Console.WriteLine("  ___/\\\\\\/____________/\\\\\\/////////\\\\\\_\\/\\\\\\//\\\\\\____/\\\\\\//\\\\\\__/\\\\\\/////////\\\\\\_      ");
            System.Console.WriteLine("   __/\\\\\\_____________\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\\\///\\\\\\/\\\\\\/_\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_     ");
            System.Console.WriteLine("    _\\/\\\\\\_____________\\/\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\_\\/\\\\\\__\\///\\\\\\/___\\/\\\\\\_\\/\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\_    ");
            System.Console.WriteLine("     _\\//\\\\\\____________\\/\\\\\\/////////\\\\\\_\\/\\\\\\____\\///_____\\/\\\\\\_\\/\\\\\\/////////\\\\\\_   ");
            System.Console.WriteLine("      __\\///\\\\\\__________\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\_____________\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_  ");
            System.Console.WriteLine("       ____\\////\\\\\\\\\\\\\\\\\\_\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\_____________\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_ ");
            System.Console.WriteLine("        _______\\/////////__\\///________\\///__\\///______________\\///__\\///________\\///__");
            System.Console.WriteLine(string.Empty);

            var app = new CommandLineApplication
            {
                Name = "Cama.Console",
                FullName = "C# mutation testing"
            };

            app.HelpOption("-?|-h|--help");

            app.Command("local", a => MutateLocalConfiguration.Configure(a));
            app.Command("git", a => MutateGitConfiguration.Configure(a));

            app.OnExecute(() => new RootCommand(app).RunAsync().Wait());

            var result = app.Execute(args);
            Environment.Exit(result);
        }
    }
}