using System;
using Cama.Core.Mutation.MutationOperators.DecisionMutations;
using Cama.Core.Mutation.MutationOperators.StatementMutations;

namespace Cama.Core.Mutation.MutationOperators
{
    public static class MutationOperatorFactory
    {
        public static IMutationOperator GetMutationOperator(Core.MutationOperators mutationOperators)
        {
            switch (mutationOperators)
            {
                case Core.MutationOperators.IfConditional:
                    return new IfConditionalMutationOperator();

                case Core.MutationOperators.SA:
                    return new BinaryExpressionMutationOperator();

                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationOperators), mutationOperators, null);
            }
        }
    }
}
