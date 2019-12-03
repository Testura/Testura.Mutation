using System.IO;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Unima.Application.Exceptions;
using Unima.Application.Models;
using Unima.Core.Config;
using Unima.Core.Git;

namespace Unima.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectExistHandler : OpenProjectHandler
    {
        private readonly IGitCloner _gitCloner;

        public OpenProjectExistHandler(IGitCloner gitCloner)
        {
            _gitCloner = gitCloner;
        }

        public override Task HandleAsync(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            var fileExist = File.Exists(fileConfig.SolutionPath);

            if (!fileExist && fileConfig.Git == null)
            {
                throw new OpenProjectException($"Could not find any solution file at {fileConfig.SolutionPath}");
            }

            if (!fileExist || (fileConfig.Git != null && fileConfig.Git.ForceClone))
            {
                LogTo.Info("Could not find project or force clone are enabled so we will clone project.");
                _gitCloner.ClonseSolution(
                    fileConfig.Git.Url,
                    fileConfig.Git.Branch,
                    fileConfig.Git.Username,
                    fileConfig.Git.Password, 
                    Path.GetDirectoryName(fileConfig.SolutionPath));
            }

            return base.HandleAsync(fileConfig, applicationConfig);
        }
    }
}
