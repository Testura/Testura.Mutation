using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Cama.Service.Commands;
using Cama.Service.Commands.Project.History.GetProjectHistory;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Services;
using Cama.Tabs;
using FluentValidation;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.Welcome
{
    public class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILoadingDisplayer _loadingDisplayer;

        public WelcomeViewModel(
            IMutationModuleTabOpener mutationModuleTabOpener,
            IStartModuleTabOpener startModuleTabOpener,
            ICommandDispatcher commandDispatcher,
            ILoadingDisplayer loadingDisplayer)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _startModuleTabOpener = startModuleTabOpener;
            _commandDispatcher = commandDispatcher;
            _loadingDisplayer = loadingDisplayer;
            CreateNewProjectCommand = new DelegateCommand(() => _startModuleTabOpener.OpenNewProjectTab());
            OpenHistoryProjectCommand = new DelegateCommand<string>(OpenHistoryProjectAsync);
            ProjectHistory = _commandDispatcher.ExecuteCommandAsync(new GetProjectHistoryCommand()).Result;
        }

        public IList<string> ProjectHistory { get; set; }

        public DelegateCommand CreateNewProjectCommand { get; set; }

        public DelegateCommand<string> OpenHistoryProjectCommand { get; set; }

        private async void OpenHistoryProjectAsync(string path)
        {
            _loadingDisplayer.ShowLoading($"Opening {Path.GetFileName(path)}");
            try
            {
                var config = await _commandDispatcher.ExecuteCommandAsync(new OpenProjectCommand(path));
                _mutationModuleTabOpener.OpenOverviewTab(config);
            }
            catch (ValidationException ex)
            {
                ErrorDialogService.ShowErrorDialog("Unexpected error", "Failed to open project.", ex.Message);
            }
            finally
            {
                _loadingDisplayer.HideLoading();
            }
        }
    }
}
