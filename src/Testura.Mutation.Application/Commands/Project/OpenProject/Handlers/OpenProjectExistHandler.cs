using System.IO;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectExistHandler : OpenProjectHandler
    {
        private readonly IGitCloner _gitCloner;

        public OpenProjectExistHandler(IGitCloner gitCloner)
        {
            _gitCloner = gitCloner;
        }

        public override async Task HandleAsync(TesturaMutationFileConfig fileConfig, TesturaMutationConfig applicationConfig)
        {
            var fileExist = File.Exists(fileConfig.SolutionPath);

            if (!fileExist && fileConfig.Git == null)
            {
                throw new OpenProjectException($"Could not find any solution file at {fileConfig.SolutionPath}");
            }

            if (!fileExist || (fileConfig.Git != null && fileConfig.Git.ForceClone))
            {
                LogTo.Info("Could not find project or force clone are enabled so we will clone project.");
                await _gitCloner.CloneSolutionAsync(
                    fileConfig.Git.RepositoryUrl,
                    fileConfig.Git.Branch,
                    fileConfig.Git.Username,
                    fileConfig.Git.Password,
                    fileConfig.Git.LocalPath);
            }

            await base.HandleAsync(fileConfig, applicationConfig);
        }
    }
}
