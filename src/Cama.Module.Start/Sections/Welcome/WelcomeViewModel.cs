using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Cama.Core.Services.Project;
using Cama.Infrastructure;
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
        private readonly IOpenCamaProjectService _openCamaProjectService;
        private readonly ILoadingDisplayer _loadingDisplayer;

        public WelcomeViewModel(
            IMutationModuleTabOpener mutationModuleTabOpener,
            IStartModuleTabOpener startModuleTabOpener,
            ProjectHistoryService projectHistoryService, 
            IOpenCamaProjectService openCamaProjectService,
            ILoadingDisplayer loadingDisplayer)
        {
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _startModuleTabOpener = startModuleTabOpener;
            _projectHistoryService = projectHistoryService;
            _openCamaProjectService = openCamaProjectService;
            _loadingDisplayer = loadingDisplayer;
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


        private async void OpenHistoryProjectAsync(string path)
        {
            _loadingDisplayer.ShowLoading($"Opening {Path.GetFileName(path)}");
            var config = await Task.Run(() => _openCamaProjectService.OpenProjectAsync(path));
            _loadingDisplayer.HideLoading();

            _mutationModuleTabOpener.OpenOverviewTab(config);
        }

    }
}
