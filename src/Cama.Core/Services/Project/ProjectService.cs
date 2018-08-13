using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;

namespace Cama.Core.Services.Project
{
    public class ProjectService : ICreateProjectService, IOpenProjectService
    {
        public void CreateProject(string savePath, CamaConfig config)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(savePath, JsonConvert.SerializeObject(config));
        }

        public async Task<CamaConfig> OpenProjectAsync(string path)
        {
            LogTo.Info($"Opening project at {path}");
            var config = JsonConvert.DeserializeObject<CamaConfig>(File.ReadAllText(path));

            MSBuildLocator.RegisterDefaults();
            var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };

            using (var workspace = MSBuildWorkspace.Create(props))
            {
                LogTo.Info("Opening solution..");
                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);
                LogTo.Info("Looking for test project output path.");
                config.TestProjectOutputPath = Path.GetDirectoryName(solution.Projects.FirstOrDefault(p => p.Name == config.TestProjectName).OutputFilePath);
                workspace.CloseSolution();
                LogTo.Info("Done opening project.");
            }

            return config;
        }
    }
}
