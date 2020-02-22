using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Util.FileSystem;

namespace Testura.Mutation.Core.Baseline
{
    public class BaselineCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaselineCreator));

        private readonly IDirectoryHandler _directoryHandler;
        private readonly BaselineCreatorCompileMutationProjectsHandler _compileMutationProjectsHandler;
        private readonly BaselineCreatorRunUnitTestsHandler _runUnitTestHandler;
        private readonly BaselineCreatorLogSummaryHandler _logSummaryHandler;

        public BaselineCreator(
            IDirectoryHandler directoryHandler,
            BaselineCreatorCompileMutationProjectsHandler compileMutationProjectsHandler,
            BaselineCreatorRunUnitTestsHandler runUnitTestHandler,
            BaselineCreatorLogSummaryHandler logSummaryHandler)
        {
            _directoryHandler = directoryHandler;
            _compileMutationProjectsHandler = compileMutationProjectsHandler;
            _runUnitTestHandler = runUnitTestHandler;
            _logSummaryHandler = logSummaryHandler;
        }

        private string BaselineDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", "Baseline");

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(MutationConfig config, CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Info("Creating baseline and verifying solution/tests..");
            _directoryHandler.DeleteDirectory(BaselineDirectoryPath);

            try
            {
                await _compileMutationProjectsHandler.CompileMutationProjectsAsync(config, BaselineDirectoryPath, cancellationToken);
                var baselineInfos = await _runUnitTestHandler.RunUnitTests(config, BaselineDirectoryPath, cancellationToken);

                _logSummaryHandler.ShowBaselineSummary(baselineInfos);

                Log.Info("Baseline completed.");
                return baselineInfos;
            }
            catch (OperationCanceledException)
            {
                Log.Info("Creating baseline was cancelled by request.");
                throw;
            }
            finally
            {
                _directoryHandler.DeleteDirectory(BaselineDirectoryPath);
            }
        }
    }
}
