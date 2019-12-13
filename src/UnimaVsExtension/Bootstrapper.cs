using Microsoft.Practices.Unity;
using Unima.Application.Extensions;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Runners;
using Unima.Core.Git;
using Unima.Core.Solution;
using Unima.Infrastructure;
using Unima.Infrastructure.Git;
using Unima.Infrastructure.Solution;
using Unima.TestRunner;

namespace UnimaVsExtension
{
    public class Bootstrapper
    {
        public static IUnityContainer GetContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterMediator(new HierarchicalLifetimeManager());
            unityContainer.RegisterType<ITestRunnerFactory, TestRunnerFactory>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IGitCloner, GitCloner>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IGitDiff, GitDIff>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISolutionBuilder, DotNetSolutionBuilder>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IProjectCompiler, Compiler>();
            unityContainer.RegisterType<IMutationDocumentCompiler, Compiler>();
            unityContainer.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            unityContainer.RegisterType<MutationToolWindow>();
            unityContainer.RegisterType<MutationToolWindowControl>();
            return unityContainer;
        }
    }
}
