using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Cama.Service.Commands.Report.Creator
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, bool>
    {
        public Task<bool> Handle(CreateReportCommand command, CancellationToken cancellationToken)
        {
            Parallel.ForEach(command.ReportCreators, reportCreator => reportCreator.SaveReport(command.Mutations));
            return Task.FromResult(true);
        }
    }
}
