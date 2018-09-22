using Cama.Service.Extensions;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;

namespace Cama.Console
{
    public class Bootstrapper
    {
        public static IUnityContainer GetContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterMediator(new HierarchicalLifetimeManager());
            return unityContainer;
        }
    }
}
