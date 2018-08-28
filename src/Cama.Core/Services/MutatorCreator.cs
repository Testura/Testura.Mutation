using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.Mutators;
using Microsoft.CodeAnalysis;
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

                foreach (var mutationprojectInfo in config.MutationProjects)
                {
                    var currentProject = solution.Projects.FirstOrDefault(p => p.Name == mutationprojectInfo.MutationProjectName);

                    if (currentProject == null)
                    {
                        LogTo.Error($"Could not find any project with the name {mutationprojectInfo.MutationProjectName}");
                        continue;
                    }

                    var documentIds = currentProject.DocumentIds;

                    LogTo.Info("Starting to create mutations..");
                    foreach (var documentId in documentIds)
                    {
                        try
                        {
                            var document = currentProject.GetDocument(documentId);

                            if (config.Filter.Any())
                            {
                                if (!config.Filter.Any(f => f.EndsWith(document.Name)))
                                {
                                    LogTo.Info($"Could not find {document.Name} in filter.. ignoring it.");
                                    continue;
                                }
                            }

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
                            var mutatedDocuments = new List<MutatedDocument>();

                            foreach (var mutationOperator in mutationOperators)
                            {
                                mutatedDocuments.AddRange(mutationOperator.GetMutatedDocument(root, document, tests));
                            }

                            if (mutatedDocuments.Any())
                            {
                                list.Add(new MFile(document.Name, mutatedDocuments));
                            }
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
