using System;
using Newtonsoft.Json;

namespace Cama.TestRunner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var testRunnerFactory = new TestRunnerFactory();

            var runner = testRunnerFactory.CreateTestRunner(args[0], args.Length > 2 ? args[2] : null);
            try
            {
                var result = runner.RunTestsAsync(args[1]).Result;
                System.Console.WriteLine(JsonConvert.SerializeObject(result));
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Message: ex.Message || Inner exception message: {ex.InnerException?.Message}");
                Environment.Exit(-1);
            }
        }
    }
}
