using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public abstract class BaselineCreatorHandler
    {
        public BaselineCreatorHandler Next { get; set; }

        public virtual Task HandleAsync(MutationConfig config, string baselineDirectoryPath, IList<BaselineInfo> baselineInfos, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Next?.HandleAsync(config, baselineDirectoryPath, baselineInfos, cancellationToken) ?? Task.CompletedTask;
        }

        public BaselineCreatorHandler SetNext(BaselineCreatorHandler handler)
        {
            Next = handler;
            return Next;
        }
    }
}
