using System.Collections.Generic;
using System.ComponentModel;
using Cama.Core.Services.Project;
using Cama.Infrastructure.Services;
using Cama.Infrastructure.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Start.Sections.Welcome
{
    class WelcomeViewModel : BindableBase, INotifyPropertyChanged
    {
        private readonly IMainTabContainer _mainTabContainer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IStartModuleTabOpener _startModuleTabOpener;
        private readonly ProjectHistoryService _projectHistoryService;
        private readonly IOpenProjectService _openProjectService;

        public WelcomeViewModel(IMutationModuleTabOpener mutationModuleTabOpener, IStartModuleTabOpener startModuleTabOpener, ProjectHistoryService projectHistoryService, IOpenProjectService openProjectService)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _startModuleTabOpener = startModuleTabOpener;
            _projectHistoryService = projectHistoryService;
            _openProjectService = openProjectService;
            ClickMeCommand = new DelegateCommand(ClickMe);
            OpenHistoryProjectCommand = new DelegateCommand<string>(OpenHistoryProjectAsync);
            ProjectHistory = _projectHistoryService.GetHistory();
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


        private async void OpenHistoryProjectAsync(string obj)
        {
            var config = await _openProjectService.OpenProjectAsync(obj);
            _mutationModuleTabOpener.OpenOverviewTab(config);
        }

    }
}
