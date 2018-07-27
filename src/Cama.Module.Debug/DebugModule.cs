using Cama.Core.Logs;
using Cama.Infrastructure;
using Cama.Module.Debug.Sections.Shell;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Cama.Module.Debug
{
    public class DebugModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public DebugModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(DebugShellView));
            _container.RegisterType<LogWatcher>(new ContainerControlledLifetimeManager());
        }
    }
}
