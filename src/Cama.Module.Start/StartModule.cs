using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace Cama.Module.Start
{
    public class StartModule : IModule
    {
        private readonly IUnityContainer _container;

        public StartModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
        }
    }
}
