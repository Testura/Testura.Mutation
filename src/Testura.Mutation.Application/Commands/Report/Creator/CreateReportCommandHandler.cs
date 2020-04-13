using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Testura.Mutation.Application.Commands.Report.Creator
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, bool>
    {
        public Task<bool> Handle(CreateReportCommand command, CancellationToken cancellationToken)
        {
            foreach (var commandReportCreator in command.ReportCreators)
            {
                commandReportCreator.SaveReport(command.Mutations, command.ExecutionTime);
            }

            return Task.FromResult(true);
        }
    }
}
