using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Unima.Application.Exceptions;
using Unima.Application.Models;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Creator.Mutators;
using Unima.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Unima.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectMutatorsHandler : OpenProjectHandler
    {
        public override Task HandleAsync(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            LogTo.Info("Loading mutators..");

            if (fileConfig.Mutators == null || !fileConfig.Mutators.Any())
            {
                LoadDefaultMutators(applicationConfig);
            }
            else
            {
                LoadCustomMutatorList(fileConfig, applicationConfig);
            }

            return base.HandleAsync(fileConfig, applicationConfig);
        }

        private void LoadCustomMutatorList(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            LogTo.Info("..found mutators in config.");
            var mutators = new List<IMutator>();
            foreach (var mutationOperator in fileConfig.Mutators)
            {
                if (Enum.TryParse<MutationOperators>(mutationOperator, true, out var mutationOperatorEnum))
                {
                    LogTo.Info($"Adding '{mutationOperator}' mutator");
                    mutators.Add(MutationOperatorFactory.GetMutationOperator(mutationOperatorEnum));
                    continue;
                }

                throw new OpenProjectException(
                    $"Could not parse '{mutationOperator}'. Make sure that you use a correct operator.");
            }

            applicationConfig.Mutators = mutators;
        }

        private void LoadDefaultMutators(UnimaConfig applicationConfig)
        {
            LogTo.Info("..did not find any mutators in config so loading default ones.");

            applicationConfig.Mutators = new List<IMutator>
            {
                new MathMutator(),
                new ConditionalBoundaryMutator(),
                new NegateConditionalMutator(),
                new ReturnValueMutator(),
                new IncrementsMutator(),
                new NegateTypeCompabilityMutator(),
            };
        }
    }
}
