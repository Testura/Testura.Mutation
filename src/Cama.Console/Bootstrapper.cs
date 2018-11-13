using Cama.Application.Extensions;
using Cama.Core.Execution.Runners;
using Cama.TestRunner;
using Microsoft.Practices.Unity;

namespace Cama.Console
{
    public class Bootstrapper
    {
        public static IUnityContainer GetContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterMediator(new HierarchicalLifetimeManager());
            unityContainer.RegisterType<ITestRunnerFactory, TestRunnerFactory>(new ContainerControlledLifetimeManager());
            return unityContainer;
        }
    }
}
