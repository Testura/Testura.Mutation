using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public class EmbeddedResourceCreator
    {
        public IEnumerable<ResourceDescription> GetManifestResources(string assemblyName, string projectPath)
        {
            var resources = new List<ResourceDescription>();
            var doc = XDocument.Load(projectPath);
            var embeddedResources = doc.Descendants().Where(d => d.Name.LocalName.Equals("EmbeddedResource", StringComparison.InvariantCultureIgnoreCase));

            foreach (var embeddedResource in embeddedResources)
            {
                var path = GetEmbeddedPath(embeddedResource);
                if (path == null)
                {
                    continue;
                }

                var resourceFullFilename = Path.Combine(Path.GetDirectoryName(projectPath), path);

                var resourceName =
                    path.EndsWith(".resx", StringComparison.OrdinalIgnoreCase) ?
                        path.Remove(path.Length - 5) + ".resources" :
                        path;

                resources.Add(new ResourceDescription(
                    $"{assemblyName}.{string.Join(".", resourceName.Split('\\'))}",
                    () => ProvideResourceData(resourceFullFilename),
                    true));
            }

            return resources;
        }

        private Stream ProvideResourceData(string resourceFullFilename)
        {
            // For non-.resx files just create a FileStream object to read the file as binary data
            if (!resourceFullFilename.EndsWith(".resx", StringComparison.OrdinalIgnoreCase))
            {
                return File.OpenRead(resourceFullFilename);
            }

            var shortLivedBackingStream = new MemoryStream();
            using (ResourceWriter resourceWriter = new ResourceWriter(shortLivedBackingStream))
            {
                resourceWriter.TypeNameConverter = TypeNameConverter;
                using (var resourceReader = new ResXResourceReader(resourceFullFilename))
                {
                    var dictionaryEnumerator = resourceReader.GetEnumerator();
                    while (dictionaryEnumerator.MoveNext())
                    {
                        if (dictionaryEnumerator.Key is string resourceKey)
                        {
                            resourceWriter.AddResource(resourceKey, dictionaryEnumerator.Value);
                        }
                    }
                }
            }

            return new MemoryStream(shortLivedBackingStream.GetBuffer());
        }

        /// <summary>
        /// This is needed to fix a "Could not load file or assembly 'System.Drawing, Version=4.0.0.0"
        /// exception, although I'm not sure why that exception was occurring.
        /// </summary>
        private string TypeNameConverter(Type objectType)
        {
            return objectType.AssemblyQualifiedName.Replace("4.0.0.0", "2.0.0.0");
        }

        private string GetEmbeddedPath(XElement embeddedResource)
        {
            var paths = embeddedResource.Attribute("Include")?.Value;
            if (paths != null)
            {
                return paths;
            }

            paths = embeddedResource.Attribute("Update")?.Value;
            return paths;
        }
    }
}
