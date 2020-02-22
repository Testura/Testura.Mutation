using System.Threading;
using System.Threading.Tasks;
using log4net;
using MediatR.Pipeline;

namespace Testura.Mutation.Application.Infrastructure
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RequestLogger<TRequest>));

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;
            Log.Info($"Testura Command: {name} {request}");

            return Task.CompletedTask;
        }
    }
}
