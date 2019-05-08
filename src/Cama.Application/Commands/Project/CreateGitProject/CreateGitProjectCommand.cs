using MediatR;

namespace Cama.Application.Commands.Project.CreateGitProject
{
    public class CreateGitProjectCommand : IRequest<bool>
    {
        public string RepositoryUrl { get; set; }

        public string Branch { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string OutputPath { get; set; }
    }
}
