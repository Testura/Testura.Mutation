using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Tests.Utils.Creators;

namespace Testura.Mutation.Tests.Core.Execution.Compiler
{
    [TestFixture]
    public class CompilerTests
    {
        [Ignore("Have to think about this one")]
        [Test]
        public async Task CompileAsync()
        {
            var configCreator = ConfigCreator.CreateConfig();
            var project = configCreator.Solution.Projects.Last();


            var compiler = new Testura.Mutation.Core.Execution.Compilation.Compiler(new EmbeddedResourceCreator());
            var result = await compiler.CompileAsync(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), project);
        }
    }
}
