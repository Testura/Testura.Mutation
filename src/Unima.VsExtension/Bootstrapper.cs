using System;
using System.Reflection;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unima.Application.Extensions;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Runners;
using Unima.Core.Git;
using Unima.Core.Solution;
using Unima.Infrastructure;
using Unima.Infrastructure.Git;
using Unima.Infrastructure.Solution;
using Unima.TestRunner;
using Unima.VsExtension.Sections.ToolsWindow;

namespace Unima.VsExtension
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
            Container.RegisterType<ISolutionBuilder, DotNetSolutionBuilder>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IProjectCompiler, Compiler>();
            Container.RegisterType<IMutationDocumentCompiler, Compiler>();
            Container.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            Container.RegisterType<MutationToolWindow>();
            Container.RegisterType<Sections.ToolsWindow.MutationToolWindowControl>();

            base.ConfigureContainer();
        }
    }
}
