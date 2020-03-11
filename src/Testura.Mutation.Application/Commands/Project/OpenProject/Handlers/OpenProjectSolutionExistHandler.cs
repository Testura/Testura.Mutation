using System.IO.Abstractions;
using System.Threading;
using Testura.Mutation.Application.Exceptions;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectSolutionExistHandler
    {
        private readonly IFileSystem _fileSystem;

        public OpenProjectSolutionExistHandler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void VerifySolutionExist(string solutionPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileExist = _fileSystem.File.Exists(solutionPath);

            if (!fileExist)
            {
                throw new OpenProjectException($"Could not find any solution file at {solutionPath}");
            }
        }
    }
}
