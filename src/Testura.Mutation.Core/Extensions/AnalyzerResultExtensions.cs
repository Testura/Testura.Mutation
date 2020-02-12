using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Testura.Mutation.Core.Extensions
{
    public static class AnalyzerResultExtensions
    {
        private static readonly char[] DirectorySplitChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        /// <summary>
        /// Gets a Roslyn workspace for the analyzed results.
        /// </summary>
        /// <param name="analyzerResult">The results from building a Buildalyzer project analyzer.</param>
        /// <param name="addProjectReferences">
        /// <c>true</c> to add projects to the workspace for project references that exist in the same <see cref="AnalyzerManager"/>.
        /// If <c>true</c> this will trigger (re)building all referenced projects. Directly add <see cref="AnalyzerResult"/> instances instead if you already have them available.
        /// </param>
        /// <returns>A Roslyn workspace.</returns>
        public static AdhocWorkspace GetWorkspace(this AnalyzerResult analyzerResult, bool addProjectReferences = false)
        {
            if (analyzerResult == null)
            {
                throw new ArgumentNullException(nameof(analyzerResult));
            }

            AdhocWorkspace workspace = new AdhocWorkspace();
            analyzerResult.AddToWorkspace(workspace, addProjectReferences);
            return workspace;
        }

        /// <summary>
        /// Adds a result to an existing Roslyn workspace.
        /// </summary>
        /// <param name="analyzerResult">The results from building a Buildalyzer project analyzer.</param>
        /// <param name="workspace">A Roslyn workspace.</param>
        /// <param name="addProjectReferences">
        /// <c>true</c> to add projects to the workspace for project references that exist in the same <see cref="AnalyzerManager"/>.
        /// If <c>true</c> this will trigger (re)building all referenced projects. Directly add <see cref="AnalyzerResult"/> instances instead if you already have them available.
        /// </param>
        /// <returns>The newly added Roslyn project.</returns>
        public static Project AddToWorkspace(this AnalyzerResult analyzerResult, Workspace workspace, bool addProjectReferences = false)
        {
            if (analyzerResult == null)
            {
                throw new ArgumentNullException(nameof(analyzerResult));
            }

            if (workspace == null)
            {
                throw new ArgumentNullException(nameof(workspace));
            }

            // Get or create an ID for this project
            var projectId = ProjectId.CreateFromSerialized(analyzerResult.ProjectGuid);

            var workspaceProjectReferfences = new ConcurrentDictionary<Guid, string[]>();

            // Cache the project references
            workspaceProjectReferfences[projectId.Id] = analyzerResult.ProjectReferences.ToArray();

            // Create and add the project
            var projectInfo = GetProjectInfo(analyzerResult, workspace, projectId);
            var solution = workspace.CurrentSolution.AddProject(projectInfo);

            // Check if this project is referenced by any other projects in the workspace
            foreach (Project existingProject in solution.Projects.ToArray())
            {
                if (!existingProject.Id.Equals(projectId)
                    && workspaceProjectReferfences.TryGetValue(existingProject.Id.Id, out string[] existingReferences)
                    && existingReferences.Contains(analyzerResult.ProjectFilePath))
                {
                    // Add the reference to the existing project
                    var projectReference = new ProjectReference(projectId);
                    solution = solution.AddProjectReference(existingProject.Id, projectReference);
                }
            }

            // Apply solution changes
            if (!workspace.TryApplyChanges(solution))
            {
                throw new InvalidOperationException("Could not apply workspace solution changes");
            }

            // Add any project references not already added
            if (addProjectReferences)
            {
                foreach (ProjectAnalyzer referencedAnalyzer in GetReferencedAnalyzerProjects(analyzerResult))
                {
                    // Check if the workspace contains the project inside the loop since adding one might also add this one due to transitive references
                    if (!workspace.CurrentSolution.Projects.Any(x => x.FilePath == referencedAnalyzer.ProjectFile.Path))
                    {
                        referencedAnalyzer.AddToWorkspace(workspace, addProjectReferences);
                    }
                }
            }

            // Find and return this project
            return workspace.CurrentSolution.GetProject(projectId);
        }

        private static ProjectInfo GetProjectInfo(AnalyzerResult analyzerResult, Workspace workspace, ProjectId projectId)
        {
            var projectName = Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath);
            var languageName = GetLanguageName(analyzerResult.ProjectFilePath);
            var projectInfo = ProjectInfo.Create(
                projectId,
                VersionStamp.Create(),
                projectName,
                projectName,
                languageName,
                filePath: analyzerResult.ProjectFilePath,
                outputFilePath: analyzerResult.GetProperty("TargetPath"),
                documents: GetDocuments(analyzerResult, projectId),
                projectReferences: GetExistingProjectReferences(analyzerResult, workspace),
                metadataReferences: GetMetadataReferences(analyzerResult),
                parseOptions: CreateParseOptions(analyzerResult, languageName),
                compilationOptions: CreateCompilationOptions(analyzerResult, languageName));
            return projectInfo;
        }

        private static ParseOptions CreateParseOptions(AnalyzerResult analyzerResult, string languageName)
        {
            if (languageName == LanguageNames.CSharp)
            {
                var parseOptions = new CSharpParseOptions();

                // Add any constants
                var constants = analyzerResult.GetProperty("DefineConstants");
                if (!string.IsNullOrWhiteSpace(constants))
                {
                    parseOptions = parseOptions
                        .WithPreprocessorSymbols(constants.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                }

                // Get language version
                string langVersion = analyzerResult.GetProperty("LangVersion");
                if (!string.IsNullOrWhiteSpace(langVersion)
                    && Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.TryParse(langVersion, out Microsoft.CodeAnalysis.CSharp.LanguageVersion languageVersion))
                {
                    parseOptions = parseOptions.WithLanguageVersion(languageVersion);
                }

                return parseOptions;
            }

            if (languageName == LanguageNames.VisualBasic)
            {
                var parseOptions = new VisualBasicParseOptions();

                // Get language version
                var langVersion = analyzerResult.GetProperty("LangVersion");
                var languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Default;
                if (!string.IsNullOrWhiteSpace(langVersion)
                    && Microsoft.CodeAnalysis.VisualBasic.LanguageVersionFacts.TryParse(langVersion, ref languageVersion))
                {
                    parseOptions = parseOptions.WithLanguageVersion(languageVersion);
                }

                return parseOptions;
            }

            return null;
        }

        private static CompilationOptions CreateCompilationOptions(AnalyzerResult analyzerResult, string languageName)
        {
            string outputType = analyzerResult.GetProperty("OutputType");
            OutputKind? kind = null;
            switch (outputType)
            {
                case "Library":
                    kind = OutputKind.DynamicallyLinkedLibrary;
                    break;
                case "Exe":
                    kind = OutputKind.ConsoleApplication;
                    break;
                case "Module":
                    kind = OutputKind.NetModule;
                    break;
                case "Winexe":
                    kind = OutputKind.WindowsApplication;
                    break;
            }

            if (kind.HasValue)
            {
                if (languageName == LanguageNames.CSharp)
                {
                    return new CSharpCompilationOptions(kind.Value);
                }

                if (languageName == LanguageNames.VisualBasic)
                {
                    return new VisualBasicCompilationOptions(kind.Value);
                }
            }

            return null;
        }

        private static IEnumerable<ProjectReference> GetExistingProjectReferences(AnalyzerResult analyzerResult, Workspace workspace) =>
            analyzerResult.ProjectReferences
                .Select(x => workspace.CurrentSolution.Projects.FirstOrDefault(y => y.FilePath == x))
                .Where(x => x != null)
                .Select(x => new ProjectReference(x.Id))
            ?? Array.Empty<ProjectReference>();

        private static IEnumerable<ProjectAnalyzer> GetReferencedAnalyzerProjects(AnalyzerResult analyzerResult) =>
            analyzerResult.ProjectReferences
                .Select(x => analyzerResult.Manager.Projects.TryGetValue(x, out ProjectAnalyzer a) ? a : null)
                .Where(x => x != null)
            ?? Array.Empty<ProjectAnalyzer>();

        private static IEnumerable<DocumentInfo> GetDocuments(AnalyzerResult analyzerResult, ProjectId projectId)
        {
            var results = new List<DocumentInfo>();

            if (analyzerResult.SourceFiles == null)
            {
                return Array.Empty<DocumentInfo>();
            }

            foreach (var file in analyzerResult.SourceFiles.Where(File.Exists))
            {
                GetDocumentNameAndFolders(file, out var name, out var folders);

                var documentInfo = DocumentInfo.Create(
                    DocumentId.CreateNewId(projectId, debugName: file),
                    name,
                    folders,
                    SourceCodeKind.Regular,
                    new FileTextLoader(file, GetEncoding(file)),
                    file,
                    false);

                results.Add(documentInfo);
            }

            return results;
        }

        private static IEnumerable<MetadataReference> GetMetadataReferences(AnalyzerResult analyzerResult) =>
            analyzerResult
                .References?.Where(File.Exists)
                .Select(x => MetadataReference.CreateFromFile(x))
            ?? (IEnumerable<MetadataReference>)Array.Empty<MetadataReference>();

        private static string GetLanguageName(string projectPath)
        {
            switch (Path.GetExtension(projectPath))
            {
                case ".csproj":
                    return LanguageNames.CSharp;
                case ".vbproj":
                    return LanguageNames.VisualBasic;
                default:
                    throw new InvalidOperationException("Could not determine supported language from project path");
            }
        }

        private static void GetDocumentNameAndFolders(string logicalPath, out string name, out ImmutableArray<string> folders)
        {
            var pathNames = logicalPath.Split(DirectorySplitChars, StringSplitOptions.RemoveEmptyEntries);
            if (pathNames.Length > 0)
            {
                if (pathNames.Length > 1)
                {
                    folders = pathNames.Take(pathNames.Length - 1).ToImmutableArray();
                }
                else
                {
                    folders = ImmutableArray<string>.Empty;
                }

                name = pathNames[pathNames.Length - 1];
            }
            else
            {
                name = logicalPath;
                folders = ImmutableArray<string>.Empty;
            }
        }

        private static Encoding GetEncoding(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                Ude.CharsetDetector cdet = new Ude.CharsetDetector();
                cdet.Feed(fs);
                cdet.DataEnd();
                if (cdet.Charset != null)
                {
                    return Encoding.GetEncoding(cdet.Charset);
                }
                else
                {
                    return Encoding.Default;
                }
            }
        }
    }
}
