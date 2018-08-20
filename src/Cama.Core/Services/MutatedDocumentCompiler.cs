using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using CompilationError = Cama.Core.Models.CompilationError;
using CompilationResult = Cama.Core.Models.CompilationResult;
using MutatedDocument = Cama.Core.Models.Mutation.MutatedDocument;

namespace Cama.Core.Services
{
    public class MutatedDocumentCompiler
    {
        public async Task<CompilationResult> CompileAsync(string path, MutatedDocument document)
        {
            var mutatedDocument = document.CreateMutatedDocument();
            var compilation = await mutatedDocument.Project.GetCompilationAsync();
            var result = compilation.Emit(path, manifestResources: GetEmbeddedResources(mutatedDocument.Project.AssemblyName, mutatedDocument.Project.FilePath));

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
                var path = embeddedResource.Attribute("Include").Value;
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