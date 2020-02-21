using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using ConsoleTables;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorLogSummaryHandler : BaselineCreatorHandler
    {
        public override Task HandleAsync(MutationConfig config, string baselineDirectoryPath, IList<BaselineInfo> baselineInfos, CancellationToken cancellationToken = default(CancellationToken))
        {
            var table = new ConsoleTable("Project", "Execution time");
            foreach (var configBaselineInfo in baselineInfos)
            {
                table.AddRow(configBaselineInfo.TestProjectName, configBaselineInfo.ExecutionTime);
            }

            LogTo.Info($"\n{table.ToStringAlternative()}");

            return base.HandleAsync(config, baselineDirectoryPath, baselineInfos, cancellationToken);
        }
    }
}
