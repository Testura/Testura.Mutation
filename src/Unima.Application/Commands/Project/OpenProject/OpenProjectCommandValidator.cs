using System.IO;
using FluentValidation;

namespace Unima.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandValidator : AbstractValidator<OpenProjectCommand>
    {
        public OpenProjectCommandValidator()
        {
            RuleFor(o => o.Path).NotEmpty().WithMessage("Path can't be empty. ");
            RuleFor(o => o.Path).NotNull().WithMessage("Path can't be null");
            RuleFor(o => o.Path).Must(File.Exists).WithMessage("File does not exist");
        }
    }
}
