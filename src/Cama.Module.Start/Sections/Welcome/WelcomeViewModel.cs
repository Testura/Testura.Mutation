using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cama.Common.Tabs;
using Prism.Commands;
using Prism.Mvvm;

namespace Cama.Module.Start.Sections.Welcome
{
    class WelcomeViewModel : BindableBase
    {
        private readonly IMainTabContainer _mainTabContainer;
        private readonly IMutationModuleTabOpener _mutationModuleTabOpener;

        public WelcomeViewModel(IMainTabContainer mainTabContainer, IMutationModuleTabOpener mutationModuleTabOpener)
        {
            _mainTabContainer = mainTabContainer;
            _mutationModuleTabOpener = mutationModuleTabOpener;
            ClickMeCommand = new DelegateCommand(ClickMe);
        }

        public DelegateCommand ClickMeCommand { get; set; }

        private void ClickMe()
        {
            _mainTabContainer.RemoveTab("Welcome");
            _mutationModuleTabOpener.OpenOverviewTab();
        }
    }
}
