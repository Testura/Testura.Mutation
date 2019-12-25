using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ValidationException = FluentValidation.ValidationException;

namespace Testura.Mutation.Application.Infrastructure
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public RequestValidationBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            var failures = _validator.Validate(context).Errors;
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}
