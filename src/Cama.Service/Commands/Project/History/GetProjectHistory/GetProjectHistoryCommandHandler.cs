using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cama.Service.Commands.Project.History.AddProjectHistory;
using Newtonsoft.Json;

namespace Cama.Service.Commands.Project.History.GetProjectHistory
{
    public class GetProjectHistoryCommandHandler : ValidateResponseRequestHandler<GetProjectHistoryCommand, IList<string>>
    {
        public override async Task<IList<string>> OnHandle(GetProjectHistoryCommand command, CancellationToken cancellationToken)
        {
            if (!File.Exists(AddProjectHistoryCommandHandler.HistoryPath))
            {
                return await Task.FromResult(new List<string>());
            }

            return await Task.FromResult(JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(AddProjectHistoryCommandHandler.HistoryPath)));
        }
    }
}
