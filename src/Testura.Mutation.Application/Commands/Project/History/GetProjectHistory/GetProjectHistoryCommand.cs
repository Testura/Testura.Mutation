using System.Collections.Generic;
using MediatR;

namespace Testura.Mutation.Application.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommand : IRequest<IList<string>>
    {
    }
}
