using System;

namespace Testura.Mutation.Core.Loggers
{
    public class MutationRunLoggerFactory : IMutationRunLoggerFactory
    {
        public IMutationRunLogger GetMutationRunLogger(MutationRunLogger mutationRunLogger)
        {
            switch (mutationRunLogger)
            {
                case MutationRunLogger.Azure:
                    return new AzureMutationRunLogger();
                default:
                    throw new ArgumentOutOfRangeException(nameof(mutationRunLogger), mutationRunLogger, null);
            }
        }
    }
}
