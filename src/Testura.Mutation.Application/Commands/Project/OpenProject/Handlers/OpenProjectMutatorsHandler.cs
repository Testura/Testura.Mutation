using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Mutators;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectMutatorsHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectMutatorsHandler));

        public void InitializeMutators(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Log.Info("Loading mutators..");

            if (fileConfig.Mutators == null || !fileConfig.Mutators.Any())
            {
                LoadDefaultMutators(applicationConfig);
            }
            else
            {
                LoadCustomMutatorList(fileConfig, applicationConfig);
            }
        }

        private void LoadCustomMutatorList(MutationFileConfig fileConfig, MutationConfig applicationConfig)
        {
            Log.Info("..found mutators in config.");
            var mutators = new List<IMutator>();
            foreach (var mutationOperator in fileConfig.Mutators)
            {
                if (Enum.TryParse<MutationOperators>(mutationOperator, true, out var mutationOperatorEnum))
                {
                    Log.Info($"Adding '{mutationOperator}' mutator");
                    mutators.Add(MutationOperatorFactory.GetMutationOperator(mutationOperatorEnum));
                    continue;
                }

                throw new OpenProjectException(
                    $"Could not parse '{mutationOperator}'. Make sure that you use a correct operator.");
            }

            applicationConfig.Mutators = mutators;
        }

        private void LoadDefaultMutators(MutationConfig applicationConfig)
        {
            Log.Info("..did not find any mutators in config so loading default ones.");

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
