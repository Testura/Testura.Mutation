using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator.Mutators;
using Testura.Mutation.Core.Creator.Mutators.BinaryExpressionMutators;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectMutatorsHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectMutatorsHandler));

        public List<IMutator> InitializeMutators(List<string> mutatorOperators, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Log.Info("Loading mutators..");

            if (mutatorOperators == null || !mutatorOperators.Any())
            {
                return LoadDefaultMutators();
            }

            return LoadCustomMutatorList(mutatorOperators);
        }

        private List<IMutator> LoadCustomMutatorList(List<string> mutatorOperators)
        {
            Log.Info("..found mutators in config.");
            var mutators = new List<IMutator>();
            foreach (var mutationOperator in mutatorOperators)
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

            return mutators;
        }

        private List<IMutator> LoadDefaultMutators()
        {
            Log.Info("..did not find any mutators in config so loading default ones.");

            return new List<IMutator>
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
