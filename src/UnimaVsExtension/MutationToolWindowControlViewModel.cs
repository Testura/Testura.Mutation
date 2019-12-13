using EnvDTE;
using MediatR;

namespace UnimaVsExtension
{
    public class MutationToolWindowControlViewModel
    {
        private readonly IMediator _mediator;
        private DTE _dte;

        public MutationToolWindowControlViewModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Do()
        {
            var solutionPath = _dte.Solution.FullName;
            /* var config = _mediator.Send(new OpenProjectCommand(configPath)).Result; */
        }

        public void Initialize(DTE getService)
        {
            _dte = getService;
        }
    }
}
