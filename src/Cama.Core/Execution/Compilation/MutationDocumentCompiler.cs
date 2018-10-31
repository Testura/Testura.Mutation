using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Anotar.Log4Net;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace Cama.Core.Execution.Compilation
{
    public class MutationDocumentCompiler
    {
        public async Task<CompilationResult> CompileAsync(string path, MutationDocument document)
        {
            LogTo.Info($"Compiling mutation {document.MutationName} to {path}");

            var mutatedDocument = await document.CreateMutatedDocumentAsync();
            var compilation = await mutatedDocument.Project.GetCompilationAsync();
            var result = compilation.Emit(path, manifestResources: GetEmbeddedResources(mutatedDocument.Project.AssemblyName, mutatedDocument.Project.FilePath));

            if (result.Success)
            {
                LogTo.Info($"Compiled {document.MutationName} successfully.");
            }
            else
            {
                var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(e => new { Location = e.Location.SourceTree.FilePath, Message = e.GetMessage() });
                LogTo.Info($"Failed to Compile {document.MutationName}: {JObject.FromObject(new { orginal = document.MutationDetails.Orginal.ToString(), mutation = document.MutationDetails.Mutation.ToString(), errors })}");
            }

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
                var path = embeddedResource.Attribute("Include")?.Value;
                if (path == null)
                {
                    continue;
                }

                var pathFixed = path.Split('\\');

                var resourcePath = Path.Combine(Path.GetDirectoryName(projectPath), path);
                resources.Add(new ResourceDescription(
                    $"{assemblyName}.{string.Join(".", pathFixed)}",
                    () => File.OpenRead(resourcePath),
                    true));
            }

            return resources;
        }
    }
}