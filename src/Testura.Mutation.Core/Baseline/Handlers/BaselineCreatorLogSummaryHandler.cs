using System.Collections.Generic;
using log4net;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorLogSummaryHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaselineCreatorLogSummaryHandler));

        public void ShowBaselineSummary(IList<BaselineInfo> baselineInfos)
        {
            Log.Info("+----- Test result -----+");

            foreach (var baselineInfo in baselineInfos)
            {
                Log.Info($"{baselineInfo.TestProjectName,-40} {baselineInfo.ExecutionTime}");
            }

            Log.Info("+----- ------------ -----+");
        }
    }
}
