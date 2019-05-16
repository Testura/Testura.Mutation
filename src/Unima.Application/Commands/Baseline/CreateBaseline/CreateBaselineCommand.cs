using System.Collections.Generic;
using MediatR;
using Unima.Core.Baseline;
using Unima.Core.Config;

namespace Unima.Application.Commands.Baseline.CreateBaseline
{
    public class CreateBaselineCommand : IRequest<IList<BaselineInfo>>
    {
        public CreateBaselineCommand(UnimaConfig config)
        {
            Config = config;
        }

        public UnimaConfig Config { get; }
    }
}
