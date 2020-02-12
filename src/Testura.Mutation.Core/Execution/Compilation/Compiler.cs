using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Anotar.Log4Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public class Compiler : IMutationDocumentCompiler, IProjectCompiler
    {
        public async Task<CompilationResult> CompileAsync(string path, MutationDocument document)
        {
            LogTo.Info($"Compiling mutation {document.MutationName} to {path}");

            var mutatedDocument = await document.CreateMutatedDocumentAsync();
            var compilation = await mutatedDocument.Project.GetCompilationAsync();
            var result = EmitCompilation(path, mutatedDocument.Project.AssemblyName, mutatedDocument.Project.FilePath, compilation);

            if (result.IsSuccess)
            {
                LogTo.Info($"Compiled {document.MutationName} successfully.");
            }
            else
            {
                var errors = result.Errors.Select(e => new { Location = e.Location,  Message = e.Message });
                LogTo.Info($"Failed to Compile {document.MutationName}: {JObject.FromObject(new { orginal = document.MutationDetails.Orginal.ToString(), mutation = document.MutationDetails.Mutation.ToString(), errors })}");
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
            var result = compilation.Emit(path, manifestResources: GetEmbeddedResources(assemblyName, filePath));

            return new CompilationResult
            {
                IsSuccess = result.Success,
                Errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => new CompilationError { Message = d.GetMessage(), Location = d.Location.ToString() })
                    .ToList()
            };
        }

        private IList<ResourceDescription> GetEmbeddedResources(string assemblyName, string projectPath)
        {
            var resources = new List<ResourceDescription>();
            var doc = XDocument.Load(projectPath);
            var embeddedResources = doc.Descendants().Where(d => d.Name.LocalName.Equals("EmbeddedResource", StringComparison.InvariantCultureIgnoreCase));
            foreach (var embeddedResource in embeddedResources)
            {
                var paths = embeddedResource.Attribute("Include")?.Value;
                if (paths == null)
                {
                    continue;
                }

                foreach (var path in paths.Split(';'))
                {
                    var pathFixed = path.Split('\\');
                    var resourcePath = Path.Combine(Path.GetDirectoryName(projectPath), path);

                    resources.Add(new ResourceDescription(
                        $"{assemblyName}.{string.Join(".", pathFixed)}",
                        () => File.OpenRead(resourcePath),
                        true));
                }
            }

            return resources;
        }
    }
}