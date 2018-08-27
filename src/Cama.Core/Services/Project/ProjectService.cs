using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models.Project;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;

namespace Cama.Core.Services.Project
{
    public class ProjectService : ICreateProjectService, IOpenProjectService
    {
        public void CreateProject(string savePath, CamaLocalConfig config)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(savePath, JsonConvert.SerializeObject(config));
        }

        public async Task<CamaRunConfig> OpenProjectAsync(string path)
        {
            LogTo.Info($"Opening project at {path}");

            var localConfig = JsonConvert.DeserializeObject<CamaLocalConfig>(File.ReadAllText(path));
            var runConfig = new CamaRunConfig { SolutionPath = localConfig.SolutionPath };

            MSBuildLocator.RegisterDefaults();
            var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };

            using (var workspace = MSBuildWorkspace.Create(props))
            {
                LogTo.Info("Opening solution..");
                var solution = await workspace.OpenSolutionAsync(localConfig.SolutionPath);
                LogTo.Info("Looking for test project output path.");

                foreach (var testProjectName in localConfig.TestProjects)
                {
                    var testProjectOutput = solution.Projects.FirstOrDefault(p => p.Name == testProjectName).OutputFilePath;
                    runConfig.TestProjects.Add(new TestProjectInfo
                    {
                        TestProjectOutputPath = Path.GetDirectoryName(testProjectOutput),
                        TestProjectOutputFileName = Path.GetFileName(testProjectOutput)
                    });
                }

                foreach (var localConfigMutationProjectName in localConfig.MutationProjects)
                {
                    runConfig.MutationProjects.Add(new MutationProjectInfo
                    {
                        MutationProjectName = localConfigMutationProjectName,
                        MutationProjectOutputFileName = Path.GetFileName(solution.Projects.FirstOrDefault(p => p.Name == localConfig.MutationProjects[0]).OutputFilePath)
                    });
                }

                workspace.CloseSolution();
                LogTo.Info("Done opening project.");
            }

            return runConfig;
        }
    }
}
