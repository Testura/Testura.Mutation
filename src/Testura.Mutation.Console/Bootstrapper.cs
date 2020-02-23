using System.IO.Abstractions;
using Microsoft.Practices.Unity;
using Testura.Mutation.Application.Extensions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Infrastructure;
using Testura.Mutation.Infrastructure.Git;
using Testura.Mutation.Infrastructure.Solution;
using Testura.Mutation.TestRunner;

namespace Testura.Mutation.Console
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
            unityContainer.RegisterType<ISolutionBuilder, DotNetSolutionBuilder>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISolutionOpener, MsBuildSolutionOpener>();
            unityContainer.RegisterType<IFileSystem, FileSystem>();
            return unityContainer;
        }
    }
}
