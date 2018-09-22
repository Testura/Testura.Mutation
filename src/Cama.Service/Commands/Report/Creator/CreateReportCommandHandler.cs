using System.Threading;
using System.Threading.Tasks;

namespace Cama.Service.Commands.Report.Creator
{
    public class CreateReportCommandHandler : ValidateResponseRequestHandler<CreateReportCommand, bool>
    {
        public override Task<bool> OnHandle(CreateReportCommand command, CancellationToken cancellationToken)
        {
            Parallel.ForEach(command.ReportCreators, reportCreator => reportCreator.SaveReport(command.Mutations));
            return Task.FromResult(true);
        }
    }
}
