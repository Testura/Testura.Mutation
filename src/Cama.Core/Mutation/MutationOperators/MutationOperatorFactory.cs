using System;
using Cama.Core.Mutation.MutationOperators.BinaryExpressionMutations;

namespace Cama.Core.Mutation.MutationOperators
{
    public static class MutationOperatorFactory
    {
        public static IMutationOperator GetMutationOperator(Core.MutationOperators mutationOperators)
        {
            switch (mutationOperators)
            {
                case Core.MutationOperators.ConditionalBoundary:
                    return new ConditionalBoundaryMutationOperator();

                case Core.MutationOperators.NegateCondtional:
                    return new NegateConditionalMutationOperator();

                case Core.MutationOperators.Math:
                    return new MathMutationOperator();

                case Core.MutationOperators.NegateTypeCompability:
                    return new NegateTypeCompabilityMutationOperator();

                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationOperators), mutationOperators, null);
            }
        }
    }
}
