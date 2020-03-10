namespace Testura.Mutation.Core.Loggers
{
    public interface IMutationRunLoggerFactory
    {
        IMutationRunLogger GetMutationRunLogger(MutationRunLogger mutationRunLogger);
    }
}