using System;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;

namespace Cama.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("________/\\\\\\\\\\\\\\\\\\_____/\\\\\\\\\\\\\\\\\\_____/\\\\\\\\____________/\\\\\\\\_____/\\\\\\\\\\\\\\\\\\____        ");
            System.Console.WriteLine(" _____/\\\\\\////////____/\\\\\\\\\\\\\\\\\\\\\\\\\\__\\/\\\\\\\\\\\\________/\\\\\\\\\\\\___/\\\\\\\\\\\\\\\\\\\\\\\\\\__       ");
            System.Console.WriteLine("  ___/\\\\\\/____________/\\\\\\/////////\\\\\\_\\/\\\\\\//\\\\\\____/\\\\\\//\\\\\\__/\\\\\\/////////\\\\\\_      ");
            System.Console.WriteLine("   __/\\\\\\_____________\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\\\///\\\\\\/\\\\\\/_\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_     ");
            System.Console.WriteLine("    _\\/\\\\\\_____________\\/\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\_\\/\\\\\\__\\///\\\\\\/___\\/\\\\\\_\\/\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\_    ");
            System.Console.WriteLine("     _\\//\\\\\\____________\\/\\\\\\/////////\\\\\\_\\/\\\\\\____\\///_____\\/\\\\\\_\\/\\\\\\/////////\\\\\\_   ");
            System.Console.WriteLine("      __\\///\\\\\\__________\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\_____________\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_  ");
            System.Console.WriteLine("       ____\\////\\\\\\\\\\\\\\\\\\_\\/\\\\\\_______\\/\\\\\\_\\/\\\\\\_____________\\/\\\\\\_\\/\\\\\\_______\\/\\\\\\_ ");
            System.Console.WriteLine("        _______\\/////////__\\///________\\///__\\///______________\\///__\\///________\\///__");
            System.Console.WriteLine("");

            LogTo.Info("Starting mutation testing");

            if(args.Length < 1)
                throw new ArgumentException("Path to cama config is required.");

            if(args.Length < 2)
                throw new ArgumentException("Output path is required.");


            MSBuildLocator.RegisterDefaults();
            var mutationRunner = Bootstrapper.GetContainer().Resolve<ConsoleMutationExecutor>();
            var success = await mutationRunner.ExecuteMutationRunner(args[0], args[1]);

            if (!success)
            {
                Environment.Exit(-1);
            }

            Environment.Exit(0);
        }
    }
}