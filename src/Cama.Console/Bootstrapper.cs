using Cama.Application.Extensions;
using Cama.Core.Execution.Compilation;
using Cama.Core.Execution.Runners;
using Cama.Infrastructure;
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
            unityContainer.RegisterType<IProjectCompiler, Compiler>();
            unityContainer.RegisterType<IMutationDocumentCompiler, Compiler>();
            unityContainer.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            return unityContainer;
        }
    }
}
