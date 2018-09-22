using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Cama.Service.Commands
{
    public abstract class ValidateResponseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        protected ValidateResponseRequestHandler(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        protected ValidateResponseRequestHandler()
        {
        }

        public async Task<TResponse> Handle(TRequest command, CancellationToken cancellationToken)
        {
            if (_validator != null)
            {
                var result = await _validator.ValidateAsync(command, cancellationToken);
                if (result.Errors.Any())
                {
                    throw new ValidationException(result.Errors);
                }
            }

            return await OnHandle(command, cancellationToken);
        }

        public abstract Task<TResponse> OnHandle(TRequest command, CancellationToken cancellationToken);
    }
}
