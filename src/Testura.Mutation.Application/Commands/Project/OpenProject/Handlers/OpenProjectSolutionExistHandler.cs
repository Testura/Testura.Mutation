using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Git;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectSolutionExistHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectSolutionExistHandler));

        private readonly IGitCloner _gitCloner;

        public OpenProjectSolutionExistHandler(IGitCloner gitCloner)
        {
            _gitCloner = gitCloner;
        }

        public async Task VerifySolutionExistAsync(MutationFileConfig fileConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileExist = File.Exists(fileConfig.SolutionPath);

            if (!fileExist && fileConfig.Git == null)
            {
                throw new OpenProjectException($"Could not find any solution file at {fileConfig.SolutionPath}");
            }

            if (!fileExist || (fileConfig.Git != null && fileConfig.Git.ForceClone))
            {
                Log.Info("Could not find project or force clone are enabled so we will clone project.");
                await _gitCloner.CloneSolutionAsync(
                    fileConfig.Git.RepositoryUrl,
                    fileConfig.Git.Branch,
                    fileConfig.Git.Username,
                    fileConfig.Git.Password,
                    fileConfig.Git.LocalPath);
            }
        }
    }
}
