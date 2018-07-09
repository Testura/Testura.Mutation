using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core.Models.Mutation;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.ReplaceFinders;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using MutatedDocument = Cama.Core.Models.Mutation.MutatedDocument;

namespace Cama.Core.Services
{
    public class SomeService
    {
        private readonly UnitTestAnalyzer _unitTestAnalyzer;

        public SomeService(UnitTestAnalyzer unitTestAnalyzer)
        {
            _unitTestAnalyzer = unitTestAnalyzer;
        }

        public async Task<ConcurrentQueue<MutatedDocument>> DoSomeWorkAsync(string solutionPath, string mainProjectName, string unitTestProjectName)
        {
            try
            {
                MSBuildLocator.RegisterDefaults();
                var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };
                var workspace = MSBuildWorkspace.Create(props);
                var solution = await workspace.OpenSolutionAsync(solutionPath);
                var testInformations = await Task.Run(() =>
                    _unitTestAnalyzer.MapTestsAsync(
                        solution.Projects.FirstOrDefault(p => p.Name == unitTestProjectName)));
                var mainProject = solution.Projects.FirstOrDefault(p => p.Name == mainProjectName);

                var queue = new ConcurrentQueue<MutatedDocument>();
                var documents = mainProject.DocumentIds;
                Parallel.ForEach(documents, documentId =>
                {
                    var document = mainProject.GetDocument(documentId);
                    var ifReplaceFinders = new IfReplaceFinder();
                    var root = document.GetSyntaxRootAsync().Result;
                    ifReplaceFinders.Visit(root);
                    var className = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier
                        .Text;
                    var tests = className != null
                        ? testInformations.Where(t => t.ReferencedClasses.Contains(className)).ToList()
                        : new List<UnitTestInformation>();

                    foreach (var replacers in ifReplaceFinders.Replacers)
                    {
                        queue.Enqueue(new MutatedDocument(document, replacers, tests));
                    }
                });

                return queue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
