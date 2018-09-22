using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core.Creator.Mutators;
using Cama.Core.Creator.Mutators.BinaryExpressionMutators;
using Cama.Core.Execution.Report;
using Cama.Core.Execution.Report.Cama;
using Cama.Core.Execution.Report.Markdown;
using Cama.Core.Execution.Report.Trx;
using Cama.Service.Commands;
using Cama.Service.Commands.Mutation.CreateMutations;
using Cama.Service.Commands.Mutation.ExecuteMutations;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Service.Commands.Report.Creator;

namespace Cama.Console
{
    public class ConsoleMutationExecutor
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public ConsoleMutationExecutor(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public async Task<bool> ExecuteMutationRunner(string configPath, string savePath)
        {
            var mutators = new List<IMutator>
            {
                new MathMutator(),
                new ConditionalBoundaryMutator(),
                new NegateConditionalMutator(),
                new ReturnValueMutator()
            };

            var config = await _commandDispatcher.ExecuteCommandAsync(new OpenProjectCommand(configPath));
            var mutationDocuments = await _commandDispatcher.ExecuteCommandAsync(new CreateMutationsCommand(config, mutators));
            var results = await _commandDispatcher.ExecuteCommandAsync(new ExecuteMutationsCommand(config, mutationDocuments, null));

            await _commandDispatcher.ExecuteCommandAsync(new CreateReportCommand(results, new List<ReportCreator>
            {
                new TrxReportCreator(savePath),
                new MarkdownReportCreator(Path.ChangeExtension(savePath, ".md")),
                new CamaReportCreator(Path.ChangeExtension(savePath, ".cama"))
            }));

            return !results.Any(r => r.Survived);
        }
    }
}
