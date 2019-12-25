using System;
using System.Reflection;
using System.Windows;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Testura.Mutation.Application.Extensions;
using Testura.Mutation.Application.Logs;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.Helpers.Displayers;
using Testura.Mutation.Infrastructure;
using Testura.Mutation.Infrastructure.Git;
using Testura.Mutation.Infrastructure.Solution;
using Testura.Mutation.Sections.Loading;
using Testura.Mutation.TestRunner;
using Testura.Mutation.Wpf.Helpers.Openers.Tabs;
using Testura.Mutation.Wpf.Sections.Shell;
using DebugShellView = Testura.Mutation.Sections.Debug.DebugShellView;
using LoadingView = Testura.Mutation.Sections.Loading.LoadingView;
using ShellWindow = Testura.Mutation.Wpf.Sections.Shell.ShellWindow;

namespace Testura.Mutation
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
            Container.RegisterType<ISolutionOpener, MsBuildSolutionOpener>();
        }
    }
}
