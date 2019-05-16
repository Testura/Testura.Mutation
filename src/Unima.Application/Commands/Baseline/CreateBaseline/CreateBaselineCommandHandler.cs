using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unima.Core.Baseline;

namespace Unima.Application.Commands.Baseline.CreateBaseline
{
    public class CreateBaselineCommandHandler : IRequestHandler<CreateBaselineCommand, IList<BaselineInfo>>
    {
        private readonly BaselineCreator _baselineCreator;

        public CreateBaselineCommandHandler(BaselineCreator baselineCreator)
        {
            _baselineCreator = baselineCreator;
        }

        public Task<IList<BaselineInfo>> Handle(CreateBaselineCommand request, CancellationToken cancellationToken)
        {
            return _baselineCreator.CreateBaselineAsync(request.Config);
        }
    }
}
