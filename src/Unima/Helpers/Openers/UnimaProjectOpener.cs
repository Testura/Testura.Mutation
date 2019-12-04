using System.IO;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Unima.Application.Commands.Project.OpenProject;
using Unima.Application.Exceptions;
using Unima.Core.Config;
using Unima.Helpers.Displayers;
using Unima.Helpers.Openers.Tabs;

namespace Unima.Helpers.Openers
{
    public class UnimaProjectOpener
    {
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IMediator _mediator;

        public UnimaProjectOpener(
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
            UnimaConfig config = null;

            try
            {
                config = await Task.Run(() => _mediator.Send(new OpenProjectCommand(path)));
                _mutationModuleTabOpener.OpenOverviewTab(config);
            }
            catch (ValidationException ex)
            {
                CommonDialogDisplayer.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.Message);
                return;
            }
            catch (OpenProjectException ex)
            {
                CommonDialogDisplayer.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.InnerException?.ToString());
                return;
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }
    }
}