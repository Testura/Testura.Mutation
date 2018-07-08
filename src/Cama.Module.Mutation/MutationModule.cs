using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace Cama.Module.Mutation
{
    public class MutationModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MutationModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
        }
    }
}
