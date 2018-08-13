using System.IO;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;
using Newtonsoft.Json;

namespace Cama.Core.Services.Project
{
    public class ProjectService : ICreateProjectService, IOpenProjectService
    {
        public void CreateProject(CamaConfig config)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(config.ProjectPath, JsonConvert.SerializeObject(config));
        }

        public CamaConfig OpenProject(string path)
        {
            LogTo.Info($"Opening project at {path}");
            var config = JsonConvert.DeserializeObject<CamaConfig>(File.ReadAllText(path));
            config.ProjectPath = path;
            return config;
        }
    }
}
