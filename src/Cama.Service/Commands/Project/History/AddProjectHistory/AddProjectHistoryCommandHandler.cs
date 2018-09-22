using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cama.Service.Commands.Project.History.AddProjectHistory
{
    public class AddProjectHistoryCommandHandler : ValidateResponseRequestHandler<AddProjectHistoryCommand, bool>
    {
        private const string FileName = "CamaProjectHistory.Json";

        public static string HistoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FileName);

        public override async Task<bool> OnHandle(AddProjectHistoryCommand command, CancellationToken cancellationToken)
        {
            var history = new List<string>();
            if (File.Exists(HistoryPath))
            {
                history = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(HistoryPath));
            }

            if (history.Contains(command.Path))
            {
                history.Remove(command.Path);
            }

            history.Insert(0, command.Path);
            File.WriteAllText(HistoryPath, JsonConvert.SerializeObject(history));

            return await Task.FromResult(true);
        }
    }
}
