using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using MediatR.Pipeline;

namespace Cama.Application.Infrastructure
{
    public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest>
    {
        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var name = typeof(TRequest).Name;
            LogTo.Info($"Camma Command: {name} {request}");

            return Task.CompletedTask;
        }
    }
}
