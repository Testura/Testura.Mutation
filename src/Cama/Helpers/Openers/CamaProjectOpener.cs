using System.IO;
using System.Threading.Tasks;
using Cama.Application.Commands.Project.OpenProject;
using Cama.Application.Exceptions;
using Cama.Helpers.Displayers;
using Cama.Helpers.Openers.Tabs;
using FluentValidation;
using MediatR;

namespace Cama.Helpers.Openers
{
    public class CamaProjectOpener
    {
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IMediator _mediator;

        public CamaProjectOpener(
            ILoadingDisplayer loadingDisplayer,
            IMutationModuleTabOpener mutationModuleTabOpener,
            IMediator mediator)
        {
            _loadingDisplayer = loadingDisplayer;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _mediator = mediator;
        }

        public async void OpenProject(string path)
        {
            _loadingDisplayer.ShowLoading($"Opening project at {Path.GetFileName(path)}");
            try
            {
                var config = await Task.Run(() => _mediator.Send(new OpenProjectCommand(path)));
                _mutationModuleTabOpener.OpenOverviewTab(config);
            }
            catch (ValidationException ex)
            {
                ErrorDialogDisplayer.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.Message);
            }
            catch (OpenProjectException ex)
            {
                ErrorDialogDisplayer.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.InnerException?.ToString());
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }
    }
}