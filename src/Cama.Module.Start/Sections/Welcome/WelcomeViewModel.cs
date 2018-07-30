using Cama.Infrastructure.Tabs;
using Cama.Module.Start.Sections.NewProject;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Start.Sections.Welcome
{
    class WelcomeViewModel : BindableBase
    {
        private readonly IMainTabContainer _mainTabContainer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;
        private readonly IStartModuleTabOpener _startModuleTabOpener;

        public WelcomeViewModel(IMainTabContainer mainTabContainer, IMutationModuleTabOpener mutationModuleTabOpener, IStartModuleTabOpener startModuleTabOpener)
        {
            _mainTabContainer = mainTabContainer;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            _startModuleTabOpener = startModuleTabOpener;
            ClickMeCommand = new DelegateCommand(ClickMe);
        }

        public DelegateCommand ClickMeCommand { get; set; }

        private async void ClickMe()
        {
            /*
            _mainTabContainer.RemoveTab("Welcome");
            _mutationModuleTabOpener.OpenOverviewTab();
            */

            _startModuleTabOpener.OpenNewProjectTab();

        }

        private void ClosingEventHandler(object sender, DialogOpenedEventArgs eventargs)
        {
        }
    }
}
