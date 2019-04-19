using System;
using System.Threading.Tasks;

namespace Cama.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            var options = CommandLineOptions.Parse(args);

            if (options?.Command == null)
            {
                // RootCommand will have printed help
                Environment.Exit(1);
            }

            Environment.Exit(await options.Command.RunAsync());
        }
    }
}