using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Testura.Mutation.Core.Baseline.Handlers;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Util.FileSystem;

namespace Testura.Mutation.Core.Baseline
{
    public class BaselineCreator
    {
        private readonly IDirectoryHandler _directoryHandler;
        private readonly BaselineCreatorHandler _handler;

        public BaselineCreator(
            IDirectoryHandler directoryHandler,
            BaselineCreatorCompileMutationProjectsHandler baselineCreatorCompileMutationProjectsHandler,
            BaselineCreatorRunUnitTestsHandler baselineCreatorRunUnitTestsHandler,
            BaselineCreatorLogSummaryHandler baselineCreatorLogSummaryHandler)
        {
            _directoryHandler = directoryHandler;
            _handler = baselineCreatorCompileMutationProjectsHandler;

            _handler
                .SetNext(baselineCreatorRunUnitTestsHandler)
                .SetNext(baselineCreatorLogSummaryHandler);
        }

        private string BaselineDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", "Baseline");

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(MutationConfig config, CancellationToken cancellationToken = default(CancellationToken))
        {
            LogTo.Info("Creating baseline and verifying solution/tests..");
            _directoryHandler.DeleteDirectory(BaselineDirectoryPath);

            try
            {
                var baselineInfos = new List<BaselineInfo>();
                await _handler.HandleAsync(config, BaselineDirectoryPath, baselineInfos, cancellationToken);

                LogTo.Info("Baseline completed.");
                return baselineInfos;
            }
            catch (OperationCanceledException)
            {
                LogTo.Info("Creating baseline was cancelled by request.");
                throw;
            }
            finally
            {
                _directoryHandler.DeleteDirectory(BaselineDirectoryPath);
            }
        }
    }
}
