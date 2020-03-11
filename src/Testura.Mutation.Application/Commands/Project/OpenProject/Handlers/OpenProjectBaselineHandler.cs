using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectBaselineHandler
    {
        private readonly BaselineCreator _baselineCreator;

        public OpenProjectBaselineHandler(BaselineCreator baselineCreator)
        {
            _baselineCreator = baselineCreator;
        }

        public async Task<IList<BaselineInfo>> RunBaselineAsync(MutationConfig config, bool runBaseline, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baselineInfos = new List<BaselineInfo>();

            if (runBaseline)
            {
                return await _baselineCreator.CreateBaselineAsync(config, cancellationToken);
            }

            return baselineInfos;
        }
    }
}
