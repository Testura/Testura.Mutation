using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR;

namespace Testura.Mutation.Application.Infrastructure
{
    public class RequestLoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger("RequestLogger");

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            Log.Info($"# Handling {typeof(TRequest).Name} #");
            var response = await next();
            Log.Info($"# Handled {typeof(TRequest).Name} #");

            return response;
        }
    }
}
