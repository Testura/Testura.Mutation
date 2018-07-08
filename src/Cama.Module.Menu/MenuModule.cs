using Cama.Common;
using Cama.Module.Menu.Sections.Menu;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Cama.Module.Menu
{
    public class MenuModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MenuModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.TopRegion, typeof(MenuView));
        }
    }
}
