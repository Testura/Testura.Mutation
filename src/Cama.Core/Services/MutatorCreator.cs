using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.Mutators;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using MutatedDocument = Cama.Core.Models.Mutation.MutatedDocument;

namespace Cama.Core.Services
{
    public class MutatorCreator
    {
        private readonly UnitTestAnalyzer _unitTestAnalyzer;

        public MutatorCreator(UnitTestAnalyzer unitTestAnalyzer)
        {
            _unitTestAnalyzer = unitTestAnalyzer;
        }

        public async Task<IList<MFile>> CreateMutatorsAsync(CamaConfig config, IList<IMutator> mutationOperators)
        {
            try
            {
                var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };
                var workspace = MSBuildWorkspace.Create(props);

                LogTo.Info("Opening solution..");
                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);
                LogTo.Info("Starting to analyze test..");

                /*
                var testInformations = await Task.Run(() =>
                    _unitTestAnalyzer.MapTestsAsync(
                        solution.Projects.FirstOrDefault(p => p.Name == unitTestProjectName).OutputFilePath,
                        solution.Projects.FirstOrDefault(p => p.Name == unitTestProjectName)));
                        */

                var list = new List<MFile>();

                foreach (var mainProjectName in config.MutationProjectNames)
                {
                    var mainProject = solution.Projects.FirstOrDefault(p => p.Name == mainProjectName);
                    var documents = mainProject.DocumentIds;

                    LogTo.Info("Starting to create mutations..");
                    for (int n = 0; n < documents.Count; n++)
                    {
                        try
                        {
                            var documentId = documents[n];
                            var document = mainProject.GetDocument(documentId);

                            LogTo.Info($"Creating mutation for {document.Name}..");

                            var root = document.GetSyntaxRootAsync().Result;
                            var className = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()
                                ?.Identifier
                                .Text;

                            /*
                            var tests = className != null
                                ? testInformations.Where(t => t.ReferencedClasses.Contains(className)).ToList()
                                : new List<UnitTestInformation>();
                                */
                            var tests = new List<UnitTestInformation>();

                            var replacers = new List<MutatedDocument>();

                            foreach (var mutationOperator in mutationOperators)
                            {
                                replacers.AddRange(mutationOperator.GetMutatedDocument(root, document, tests));
                            }

                            list.Add(new MFile(document.Name, replacers));
                        }
                        catch (Exception ex)
                        {
                            LogTo.Error("Error when creating mutation: " + ex.Message);
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
