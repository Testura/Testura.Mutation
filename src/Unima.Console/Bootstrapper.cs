using Microsoft.Practices.Unity;
using Unima.Application.Extensions;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Runners;
using Unima.Core.Git;
using Unima.Infrastructure;
using Unima.Infrastructure.Git;
using Unima.TestRunner;

namespace Unima.Console
{
    public class Bootstrapper
    {
        public static IUnityContainer GetContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterMediator(new HierarchicalLifetimeManager());
            unityContainer.RegisterType<ITestRunnerFactory, TestRunnerFactory>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IGitCloner, GitCloner>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IProjectCompiler, Compiler>();
            unityContainer.RegisterType<IMutationDocumentCompiler, Compiler>();
            unityContainer.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            return unityContainer;
        }
    }
}
