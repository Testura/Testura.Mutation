using FluentValidation;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandValidator : AbstractValidator<ExecuteMutationsCommand>
    {
        public ExecuteMutationsCommandValidator()
        {
            RuleFor(command => command.Config).NotNull();
            RuleFor(command => command.MutationDocuments).NotNull();
            RuleFor(command => command.Config.NumberOfTestRunInstances).GreaterThan(0);
        }
    }
}
