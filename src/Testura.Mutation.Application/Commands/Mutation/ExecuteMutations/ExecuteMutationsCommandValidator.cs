using FluentValidation;
using FluentValidation.Results;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutations
{
    public class ExecuteMutationsCommandValidator : AbstractValidator<ExecuteMutationsCommand>
    {
        public ExecuteMutationsCommandValidator()
        {
            RuleFor(command => command.MutationDocuments).NotNull();
            RuleFor(command => command.Config.NumberOfTestRunInstances).GreaterThan(0);
            RuleFor(command => command.Config.TestProjects).NotEmpty().WithMessage("At least one test project is required.");
            RuleFor(command => command.Config.Solution).NotNull();
        }

        protected override bool PreValidate(ValidationContext<ExecuteMutationsCommand> context, ValidationResult result)
        {
            if (context.InstanceToValidate.Config == null)
            {
                result.Errors.Add(new ValidationFailure("Config", "Config can not be null."));
                return false;
            }

            return true;
        }
    }
}
