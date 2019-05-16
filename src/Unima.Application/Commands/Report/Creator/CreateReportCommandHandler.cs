using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Unima.Application.Commands.Report.Creator
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, bool>
    {
        public Task<bool> Handle(CreateReportCommand command, CancellationToken cancellationToken)
        {
            Parallel.ForEach(command.ReportCreators, reportCreator => reportCreator.SaveReport(command.Mutations, command.ExecutionTime));
            return Task.FromResult(true);
        }
    }
}
