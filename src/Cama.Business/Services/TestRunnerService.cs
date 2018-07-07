using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cama.Business.Mutation;
using Microsoft.CodeAnalysis;
using Testura.Module.TestRunner;
using Testura.Module.TestRunner.Result;

namespace Cama.Business.Services
{
    public class TestRunnerService
    {
        public async Task<NUnitTestSuiteResult> RunTestsAsync(MutatedDocument mutatedDocument)
        {
            var compilation = await mutatedDocument.CreateMutatedDocument().Project.GetCompilationAsync();
            Directory.CreateDirectory(Path.Combine("TestRun", mutatedDocument.Id.ToString()));
            var result = compilation.Emit(Path.Combine("TestRun", mutatedDocument.Id.ToString(), "Testura.Code.dll"));
            if (result.Success)
            {
                var files = Directory.GetFiles(@"D:\Programmering\Testura\Testura.Code\src\Testura.Code.Tests\bin\Debug");
                foreach (var file in files.Where(f => f.EndsWith(".dll") && !f.Contains("Testura.Code.dll")))
                {
                    File.Copy(file, Path.Combine("TestRun", mutatedDocument.Id.ToString(), Path.GetFileName(file)), true);
                }

                var testDll = @"D:\Programmering\Testura\Testura.Code\src\Testura.Code.Tests\bin\Debug\Testura.Code.Tests.dll";
                var testRunner = new TestRunner();
                var results = testRunner.RunTests(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", mutatedDocument.Id.ToString(), Path.GetFileName(testDll)), mutatedDocument.Tests);

                Directory.Delete(Path.Combine("TestRun", mutatedDocument.Id.ToString()), true);

                return results;
            }

            return null;
        }
    }
}
