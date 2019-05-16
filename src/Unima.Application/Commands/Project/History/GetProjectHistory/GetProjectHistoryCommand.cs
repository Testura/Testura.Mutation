using System.Collections.Generic;
using MediatR;

namespace Unima.Application.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommand : IRequest<IList<string>>
    {
    }
}
