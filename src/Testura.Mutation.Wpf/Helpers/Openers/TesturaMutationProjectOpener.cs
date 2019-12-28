using System.IO;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Testura.Mutation.Application.Commands.Project.OpenProject;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Helpers.Displayers;
using Testura.Mutation.Wpf.Helpers.Openers.Tabs;

namespace Testura.Mutation.Wpf.Helpers.Openers
{
    public class TesturaMutationProjectOpener
    {
        private readonly ILoadingDisplayer _loadingDisplayer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IMediator _mediator;

        public TesturaMutationProjectOpener(
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
            MutationConfig config = null;

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