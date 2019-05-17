using System;
using Newtonsoft.Json;

namespace Unima.TestRunner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var testRunnerFactory = new TestRunnerFactory();

            var runner = testRunnerFactory.CreateTestRunner(args[0], TimeSpan.Parse(args[2]), args.Length > 3 ? args[3] : null);
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
