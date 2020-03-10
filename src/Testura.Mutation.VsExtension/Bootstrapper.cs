using System;
using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity;
using Testura.Mutation.Application.Extensions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Core.Loggers;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Infrastructure;
using Testura.Mutation.Infrastructure.Git;
using Testura.Mutation.TestRunner;
using Testura.Mutation.VsExtension.Sections.Config;
using Testura.Mutation.VsExtension.Sections.MutationExplorer;

namespace Testura.Mutation.VsExtension
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureServiceLocator()
        {
            base.ConfigureServiceLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                if (viewName.Contains("WindowControl"))
                {
                    viewName = viewName.Remove(viewName.LastIndexOf("Control"));
                }
                else if (viewName.Contains("Window"))
                {
                    viewName = viewName.Remove(viewName.LastIndexOf("Window"));
                }
                else if (viewName.Contains("Dialog"))
                {
                    viewName = viewName.Remove(viewName.LastIndexOf("Dialog"));
                }
                else if (viewName.Contains("View"))
                {
                    viewName = viewName.Remove(viewName.LastIndexOf("View"));
                }

                viewName = viewName.Replace("View", "ViewModel");
                var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }

        protected override void ConfigureContainer()
        {
            Container.RegisterMediator(new HierarchicalLifetimeManager());
            Container.RegisterType<ITestRunnerFactory, TestRunnerFactory>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IGitCloner, GitCloner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IGitDiff, GitDIff>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IProjectCompiler, Compiler>();
            Container.RegisterType<IMutationDocumentCompiler, Compiler>();
            Container.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            Container.RegisterType<MutationExplorerWindow>();
            Container.RegisterType<MutationExplorerWindowControl>();
            Container.RegisterType<MutationConfigWindow>();
            Container.RegisterType<MutationConfigWindowControl>();
            Container.RegisterType<ISolutionOpener, MsBuildSolutionOpener>();
            Container.RegisterType<IFileSystem, FileSystem>();
            Container.RegisterType<IMutationRunLoggerManager, MutationRunLoggerManager>();

            base.ConfigureContainer();
        }
    }
}
