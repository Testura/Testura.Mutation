using System;
using Cama.Core.Mutators.BinaryExpressionMutators;

namespace Cama.Core.Mutators
{
    public static class MutationOperatorFactory
    {
        public static IMutator GetMutationOperator(Core.MutationOperators mutationOperators)
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

                case MutationOperators.ReturnValue:
                    return new ReturnValueMutator();

                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationOperators), mutationOperators, null);
            }
        }
    }
}
