using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Cama.Service.Commands;
using Cama.Service.Commands.Project.History.GetProjectHistory;
using Cama.Service.Commands.Project.OpenProject;
using Cama.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Sections.Welcome
{
    public class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMainTabContainer _mainTabContainer;
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
            ClickMeCommand = new DelegateCommand(ClickMe);
            OpenHistoryProjectCommand = new DelegateCommand<string>(OpenHistoryProjectAsync);
            ProjectHistory = _commandDispatcher.ExecuteCommandAsync(new GetProjectHistoryCommand()).Result;
        }

        public IList<string> ProjectHistory { get; set; }

        public DelegateCommand ClickMeCommand { get; set; }

        public DelegateCommand<string> OpenHistoryProjectCommand { get; set; }

        private async void ClickMe()
        {
            /*
            _mainTabContainer.RemoveTab("Welcome");
            _mutationModuleTabOpener.OpenOverviewTab();
            */

            _startModuleTabOpener.OpenNewProjectTab();

        }

        private async void OpenHistoryProjectAsync(string path)
        {
            _loadingDisplayer.ShowLoading($"Opening {Path.GetFileName(path)}");
            var config = await _commandDispatcher.ExecuteCommandAsync(new OpenProjectCommand(path));
            _loadingDisplayer.HideLoading();

            _mutationModuleTabOpener.OpenOverviewTab(config);
        }

    }
}
