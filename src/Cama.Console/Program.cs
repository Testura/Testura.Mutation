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
            System.Console.WriteLine("   ____    _    __  __    _  ");
            System.Console.WriteLine("  / ___|  / \\  |  \\/  |  / \\");
            System.Console.WriteLine(" | |     / _ \\ | |\\/| | / _ \\ ");
            System.Console.WriteLine(" | |___ / ___ \\| |  | |/ ___ \\ ");
            System.Console.WriteLine("  \\____/_/   \\_\\_|  |_/_/   \\_\\");
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