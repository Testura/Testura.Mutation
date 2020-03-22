using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public class Compiler : IMutationDocumentCompiler, IProjectCompiler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Compiler));
        private readonly EmbeddedResourceCreator _embeddedResourceCreator;

        public Compiler(EmbeddedResourceCreator embeddedResourceCreator)
        {
            _embeddedResourceCreator = embeddedResourceCreator;
        }

        public async Task<CompilationResult> CompileAsync(string path, MutationDocument document)
        {
            Log.Info($"Compiling mutation {document.MutationName} to {path}");

            var mutatedDocument = await document.CreateMutatedDocumentAsync();
            var compilation = await mutatedDocument.Project.GetCompilationAsync();
            var result = EmitCompilation(path, mutatedDocument.Project.AssemblyName, mutatedDocument.Project.FilePath, compilation);

            if (result.IsSuccess)
            {
                Log.Info($"Compiled {document.MutationName} successfully.");
            }
            else
            {
                var errors = result.Errors.Select(e => new { Location = e.Location,  Message = e.Message });
                Log.Info($"Failed to Compile {document.MutationName}: {JObject.FromObject(new { orginal = document.MutationDetails.Orginal.ToString(), mutation = document.MutationDetails.Mutation.ToString(), errors })}");
            }

            return result;
        }

        public async Task<CompilationResult> CompileAsync(string directoryPath, Project project)
        {
            var path = Path.Combine(directoryPath, Path.GetFileName(project.OutputFilePath));
            var compilation = await project.GetCompilationAsync();
            var result = EmitCompilation(path, project.AssemblyName, project.FilePath, compilation);
            return result;
        }

        private CompilationResult EmitCompilation(
            string path,
            string assemblyName,
            string filePath,
            Microsoft.CodeAnalysis.Compilation compilation)
        {
            var result = compilation.Emit(path, manifestResources: _embeddedResourceCreator.GetManifestResources(assemblyName, filePath));

            return new CompilationResult
            {
                IsSuccess = result.Success,
                Errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => new CompilationError { Message = d.GetMessage(), Location = d.Location.ToString() })
                    .ToList()
            };
        }
    }
}