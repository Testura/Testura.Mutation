using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;

namespace Testura.Mutation.Application.Commands.Project.History.AddProjectHistory
{
    public class AddProjectHistoryCommandHandler : IRequestHandler<AddProjectHistoryCommand, bool>
    {
        private const string FileName = "Testura.MutationProjectHistory.Json";

        public static string HistoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), FileName);

        public async Task<bool> Handle(AddProjectHistoryCommand command, CancellationToken cancellationToken)
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
