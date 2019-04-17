using System;
using Newtonsoft.Json;

namespace Cama.TestRunner.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var testRunnerFactory = new TestRunnerFactory();

            var runner = testRunnerFactory.CreateTestRunner(args[0], args[1]);
            try
            {
                var result = runner.RunTestsAsync(args[2]).Result;
                System.Console.WriteLine(JsonConvert.SerializeObject(result));
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }
    }
}
