using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Util.FileSystem;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorCompileMutationProjectsHandler
    {
        private readonly IProjectCompiler _projectCompiler;
        private readonly IDirectoryHandler _directoryHandler;

        public BaselineCreatorCompileMutationProjectsHandler(IProjectCompiler projectCompiler, IDirectoryHandler directoryHandler)
        {
            _projectCompiler = projectCompiler;
            _directoryHandler = directoryHandler;
        }

        public async Task CompileMutationProjects(MutationConfig config, string baselineDirectoryPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            _directoryHandler.CreateDirectory(baselineDirectoryPath);

            foreach (var mutationProject in config.MutationProjects)
            {
                cancellationToken.ThrowIfCancellationRequested();

                LogTo.Info($"Compiling {mutationProject.Project.Name}..");

                var project = config.Solution.Projects.FirstOrDefault(p => p.Name == mutationProject.Project.Name);
                var result = await _projectCompiler.CompileAsync(baselineDirectoryPath, project);

                if (!result.IsSuccess)
                {
                    foreach (var compilationError in result.Errors)
                    {
                        LogTo.Error($"{{ Error = \"{compilationError.Message}\", Location = \"{compilationError.Location}\"");
                    }

                    throw new BaselineException(
                        $"Failed to compile {project.Name} in base line.",
                        new CompilationException(result.Errors.Select(e => e.Message)));
                }
            }
        }
    }
}
