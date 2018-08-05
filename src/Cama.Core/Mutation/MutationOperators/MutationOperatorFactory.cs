using System;
using Cama.Core.Mutation.MutationOperators.DecisionMutations;

namespace Cama.Core.Mutation.MutationOperators
{
    public static class MutationOperatorFactory
    {
        public static IMutationOperator GetMutationOperator(Core.MutationOperators mutationOperators)
        {
            switch (mutationOperators)
            {
                case Core.MutationOperators.DIOC:
                    return new DicMutationOperator();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationOperators), mutationOperators, null);
            }
        }
    }
}
