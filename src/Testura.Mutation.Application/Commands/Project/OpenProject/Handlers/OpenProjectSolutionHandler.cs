using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectSolutionHandler
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISolutionOpener _solutionOpener;

        public OpenProjectSolutionHandler(IFileSystem fileSystem, ISolutionOpener solutionOpener)
        {
            _fileSystem = fileSystem;
            _solutionOpener = solutionOpener;
        }

        public async Task<Microsoft.CodeAnalysis.Solution> OpenSolutionAsync(string solutionPath, string buildConfiguration, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileExist = _fileSystem.File.Exists(solutionPath);

            if (!fileExist)
            {
                throw new OpenProjectException($"Could not find any solution file at {solutionPath}");
            }

            return await _solutionOpener.GetSolutionAsync(solutionPath, buildConfiguration, true);
        }
    }
}
