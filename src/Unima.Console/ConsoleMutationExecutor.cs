using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Unima.Application.Commands.Mutation.CreateMutations;
using Unima.Application.Commands.Mutation.ExecuteMutations;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Commands.Report.Creator;
using Unima.Core.Execution.Report;
using Unima.Core.Execution.Report.Html;
using Unima.Core.Execution.Report.Markdown;
using Unima.Core.Execution.Report.Summary;
using Unima.Core.Execution.Report.Trx;
using Unima.Core.Execution.Report.Unima;

namespace Unima.Console
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
            var config = await _mediator.Send(new OpenProjectCommand(configPath, true));

            var start = DateTime.Now;
            var mutationDocuments = await _mediator.Send(new CreateMutationsCommand(config));
            var results = await _mediator.Send(new ExecuteMutationsCommand(config, mutationDocuments.ToList(), null));

            var trxSavePath = Path.Combine(savePath, "result.trx");
            var reports = new List<ReportCreator>
            {
                new TrxReportCreator(trxSavePath),
                new MarkdownReportCreator(Path.ChangeExtension(trxSavePath, ".md")),
                new UnimaReportCreator(Path.ChangeExtension(trxSavePath, ".unima")),
                new HtmlOnlyBodyReportCreator(Path.ChangeExtension(trxSavePath, ".html")),
                new TextSummaryReportCreator(Path.ChangeExtension(trxSavePath, ".txt"))
            };

            await _mediator.Send(new CreateReportCommand(results, reports, DateTime.Now - start));

            return !results.Any(r => r.Survived);
        }
    }
}
