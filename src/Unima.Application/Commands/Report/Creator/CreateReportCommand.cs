using System;
using System.Collections.Generic;
using MediatR;
using Unima.Core;
using Unima.Core.Execution.Report;

namespace Unima.Application.Commands.Report.Creator
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
