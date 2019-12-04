using System;
using System.Reflection;
using System.Windows;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Unima.Application.Extensions;
using Unima.Application.Logs;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Runners;
using Unima.Core.Git;
using Unima.Core.Solution;
using Unima.Helpers.Displayers;
using Unima.Helpers.Openers.Tabs;
using Unima.Infrastructure;
using Unima.Infrastructure.Git;
using Unima.Infrastructure.Solution;
using Unima.Sections.Loading;
using Unima.Sections.Shell;
using Unima.TestRunner;
using DebugShellView = Unima.Sections.Debug.DebugShellView;
using LoadingView = Unima.Sections.Loading.LoadingView;
using ShellWindow = Unima.Sections.Shell.ShellWindow;

namespace Unima
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
                if (viewName.Contains("Window"))
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

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.LoadRegion, typeof(LoadingView));
            regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(DebugShellView));
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            MSBuildLocator.RegisterDefaults();
            System.Windows.Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterMediator(new HierarchicalLifetimeManager());
            Container.RegisterType<IMainTabContainer, TabContainer>();
            Container.RegisterType<IMutationModuleTabOpener, MutationTabOpener>();
            Container.RegisterType<IStartModuleTabOpener, StartModuleTabOpener>();
            Container.RegisterType<LogWatcher>(new ContainerControlledLifetimeManager());
            Container.RegisterType<LoadingViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITestRunnerFactory, TestRunnerFactory>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILoadingDisplayer, LoadingViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IProjectCompiler, Compiler>();
            Container.RegisterType<IMutationDocumentCompiler, Compiler>();
            Container.RegisterType<ITestRunnerClient, TestRunnerConsoleClient>();
            Container.RegisterType<IGitCloner, GitCloner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IGitDiff, GitDIff>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ISolutionBuilder, DotNetSolutionBuilder>(new ContainerControlledLifetimeManager());
        }
    }
}
