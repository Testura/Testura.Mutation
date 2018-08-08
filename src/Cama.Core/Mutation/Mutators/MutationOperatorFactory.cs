using System;
using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;

namespace Cama.Core.Mutation.Mutators
{
    public static class MutationOperatorFactory
    {
        public static IMutationOperator GetMutationOperator(Core.MutationOperators mutationOperators)
        {
            switch (mutationOperators)
            {
                case MutationOperators.ConditionalBoundary:
                    return new ConditionalBoundaryMutator();

                case MutationOperators.NegateCondtional:
                    return new NegateConditionalMutator();

                case MutationOperators.Math:
                    return new MathMutator();

                case MutationOperators.NegateTypeCompability:
                    return new NegateTypeCompabilityMutator();

                case MutationOperators.Increment:
                    return new IncrementsMutator();

                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationOperators), mutationOperators, null);
            }
        }
    }
}
