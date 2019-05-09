using System.Collections.Generic;
using Cama.Core.Baseline;
using Cama.Core.Config;
using MediatR;

namespace Cama.Application.Commands.Baseline.CreateBaseline
{
    public class CreateBaselineCommand : IRequest<IList<BaselineInfo>>
    {
        public CreateBaselineCommand(CamaConfig config)
        {
            Config = config;
        }

        public CamaConfig Config { get; }
    }
}
