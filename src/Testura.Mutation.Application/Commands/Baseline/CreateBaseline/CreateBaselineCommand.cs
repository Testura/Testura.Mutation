﻿using System.Collections.Generic;
using MediatR;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Baseline.CreateBaseline
{
    public class CreateBaselineCommand : IRequest<IList<BaselineInfo>>
    {
        public CreateBaselineCommand(TesturaMutationConfig config)
        {
            Config = config;
        }

        public TesturaMutationConfig Config { get; }
    }
}
