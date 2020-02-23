using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution.Compilation;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorCompileMutationProjectsHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaselineCreatorCompileMutationProjectsHandler));

        private readonly IProjectCompiler _projectCompiler;
        private readonly IFileSystem _fileSystem;

        public BaselineCreatorCompileMutationProjectsHandler(IProjectCompiler projectCompiler, IFileSystem fileSystem)
        {
            _projectCompiler = projectCompiler;
            _fileSystem = fileSystem;
        }

        public async Task CompileMutationProjectsAsync(MutationConfig config, string baselineDirectoryPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            _fileSystem.Directory.CreateDirectory(baselineDirectoryPath);

            foreach (var mutationProject in config.MutationProjects)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var project = config.Solution.Projects.FirstOrDefault(p => p.Name == mutationProject.Project.Name);
                if (project == null)
                {
                    throw new BaselineException($"Could not find any project with the name {mutationProject.Project.Name} " +
                                                $"in the solution. Currently the solution have these projects: {string.Join(", ", config.Solution.Projects.Select(p => p.Name))}");
                }

                Log.Info($"Starting to compile {project.Name}..");

                var result = await _projectCompiler.CompileAsync(baselineDirectoryPath, project);

                if (!result.IsSuccess)
                {
                    Log.Info($".. compiled failed.");

                    foreach (var compilationError in result.Errors)
                    {
                        Log.Error($"{{ Error = \"{compilationError.Message}\", Location = \"{compilationError.Location}\"");
                    }

                    throw new BaselineException(
                        $"Failed to compile {project.Name} in base line.",
                        new CompilationException(result.Errors.Select(e => e.Message)));
                }

                Log.Info($".. compiled successfully.");
            }
        }
    }
}
