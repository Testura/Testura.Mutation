using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cama.Application.Commands.Mutation.CreateMutations;
using Cama.Application.Commands.Mutation.ExecuteMutations;
using Cama.Application.Commands.Project.OpenProject;
using Cama.Application.Commands.Report.Creator;
using Cama.Core.Creator.Mutators;
using Cama.Core.Creator.Mutators.BinaryExpressionMutators;
using Cama.Core.Execution.Report;
using Cama.Core.Execution.Report.Cama;
using Cama.Core.Execution.Report.Html;
using Cama.Core.Execution.Report.Markdown;
using Cama.Core.Execution.Report.Trx;
using MediatR;

namespace Cama.Console
{
    public class ConsoleMutationExecutor
    {
        private readonly IMediator _mediator;

        public ConsoleMutationExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> ExecuteMutationRunner(string configPath, string savePath)
        {
            var mutators = new List<IMutator>
            {
                new MathMutator(),
                new ConditionalBoundaryMutator(),
                new NegateConditionalMutator(),
                new ReturnValueMutator(),
                new IncrementsMutator(),
                new NegateTypeCompabilityMutator()
            };

            var config = await _mediator.Send(new OpenProjectCommand(configPath));
            var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(config, mutators));
            var results = await _mediator.Send(new ExecuteMutationsCommand(config, mutationDocuments.Take(4).ToList(), null));
            var reports = new List<ReportCreator>
            {
                new TrxReportCreator(savePath),
                new MarkdownReportCreator(Path.ChangeExtension(savePath, ".md")),
                new CamaReportCreator(Path.ChangeExtension(savePath, ".cama")),
                new HtmlOnlyBodyReportCreator(Path.ChangeExtension(savePath, ".html"))
            };

            await _mediator.Send(new CreateReportCommand(results, reports));

            return !results.Any(r => r.Survived);
        }
    }
}
