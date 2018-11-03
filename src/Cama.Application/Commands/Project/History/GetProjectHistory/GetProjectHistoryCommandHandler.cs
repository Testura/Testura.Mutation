using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cama.Application.Commands.Project.History.AddProjectHistory;
using MediatR;
using Newtonsoft.Json;

namespace Cama.Application.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommandHandler : IRequestHandler<GetProjectHistoryCommand, IList<string>>
    {
        public async Task<IList<string>> Handle(GetProjectHistoryCommand command, CancellationToken cancellationToken)
        {
            if (!File.Exists(AddProjectHistoryCommandHandler.HistoryPath))
            {
                return await Task.FromResult(new List<string>());
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(AddProjectHistoryCommandHandler.HistoryPath)));
        }
    }
}
