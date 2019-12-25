using MediatR;

namespace Testura.Mutation.Application.Commands.Project.History.AddProjectHistory
{
    public class AddProjectHistoryCommand : IRequest<bool>
    {
        public AddProjectHistoryCommand(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
