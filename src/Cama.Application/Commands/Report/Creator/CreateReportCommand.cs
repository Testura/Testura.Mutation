using System.Collections.Generic;
using Cama.Core;
using Cama.Core.Execution.Report;
using MediatR;

namespace Cama.Application.Commands.Report.Creator
{
    public class CreateReportCommand : IRequest<bool>
    {
        public CreateReportCommand(IList<MutationDocumentResult> mutations, IList<ReportCreator> reportCreators)
        {
            Mutations = mutations;
            ReportCreators = reportCreators;
        }

        public IList<MutationDocumentResult> Mutations { get; set; }

        public IList<ReportCreator> ReportCreators { get; set; }
    }
}
