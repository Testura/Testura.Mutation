using System;
using System.Collections.Generic;
using Cama.Core;
using Cama.Core.Execution.Report;
using MediatR;

namespace Cama.Application.Commands.Report.Creator
{
    public class CreateReportCommand : IRequest<bool>
    {
        public CreateReportCommand(IList<MutationDocumentResult> mutations, IList<ReportCreator> reportCreators, TimeSpan executionTime)
        {
            Mutations = mutations;
            ReportCreators = reportCreators;
            ExecutionTime = executionTime;
        }

        public IList<MutationDocumentResult> Mutations { get; set; }

        public IList<ReportCreator> ReportCreators { get; set; }

        public TimeSpan ExecutionTime { get; }
    }
}
