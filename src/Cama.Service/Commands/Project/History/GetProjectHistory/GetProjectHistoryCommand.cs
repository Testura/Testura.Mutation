using System.Collections.Generic;
using MediatR;

namespace Cama.Service.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommand : IRequest<IList<string>>
    {
    }
}
