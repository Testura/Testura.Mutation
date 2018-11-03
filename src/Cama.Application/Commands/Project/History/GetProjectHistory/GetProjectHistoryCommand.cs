using System.Collections.Generic;
using MediatR;

namespace Cama.Application.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommand : IRequest<IList<string>>
    {
    }
}
