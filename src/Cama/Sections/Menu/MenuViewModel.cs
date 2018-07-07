using System;
using Cama.Common;
using Cama.Module.Mutation;
using Cama.Module.Mutation.Sections.Shell;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

namespace Cama.Sections.Menu
{
    public class MenuViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IModuleManager _moduleManager;

        public MenuViewModel(IRegionManager regionManager, IModuleManager moduleManager)
        {
            _regionManager = regionManager;
            _moduleManager = moduleManager;
            NewProjectCommand = new DelegateCommand(NewProject);
        }

        public DelegateCommand NewProjectCommand { get; set; }

        private void NewProject()
        {
            _moduleManager.LoadModule(nameof(MutationModule));
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, new Uri(typeof(MutationShellView).FullName, UriKind.Relative));
        }

    }
}
