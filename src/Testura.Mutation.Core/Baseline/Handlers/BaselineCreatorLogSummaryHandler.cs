using System.Collections.Generic;
using Anotar.Log4Net;
using ConsoleTables;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorLogSummaryHandler
    {
        public void ShowBaselineSummary(IList<BaselineInfo> baselineInfos)
        {
            var table = new ConsoleTable("Project", "Execution time");
            foreach (var configBaselineInfo in baselineInfos)
            {
                table.AddRow(configBaselineInfo.TestProjectName, configBaselineInfo.ExecutionTime);
            }

            LogTo.Info($"\n{table.ToStringAlternative()}");
        }
    }
}
