using System.Collections.Generic;
using log4net;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorLogSummaryHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaselineCreatorLogSummaryHandler));

        public void ShowBaselineSummary(IList<BaselineInfo> baselineInfos)
        {
            /*
            var table = new ConsoleTable("Project", "Execution time");
            foreach (var configBaselineInfo in baselineInfos)
            {
                table.AddRow(configBaselineInfo.TestProjectName, configBaselineInfo.ExecutionTime);
            }

            Log.Info($"\n{table.ToStringAlternative()}");
            */

            Log.Info("TEMPORARY");
        }
    }
}
