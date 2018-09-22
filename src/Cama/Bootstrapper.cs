using System;
using System.Reflection;
using System.Windows;
using Cama.Sections.Loading;
using Cama.Sections.Shell;
using Cama.Service.Extensions;
using Cama.Service.Logs;
using Cama.Tab;
using Cama.Tabs;
using Microsoft.Build.Locator;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using DebugShellView = Cama.Sections.Debug.DebugShellView;
using LoadingView = Cama.Sections.Loading.LoadingView;
using ShellWindow = Cama.Sections.Shell.ShellWindow;

namespace Cama
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
            Application.Current.MainWindow.Show();
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
            Container.RegisterType<ILoadingDisplayer, LoadingViewModel>(new ContainerControlledLifetimeManager());
        }
    }
}
