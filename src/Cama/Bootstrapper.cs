using System;
using System.Reflection;
using System.Windows;
using Cama.Module.Mutation;
using Cama.Module.Start;
using Cama.Sections.Shell;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity;

namespace Cama
{
    class Bootstrapper : UnityBootstrapper
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
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var startModule = typeof(StartModule);
            var mutationModule = typeof(MutationModule);

            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = startModule.Name,
                ModuleType = startModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.WhenAvailable
            });

            ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = mutationModule.Name,
                ModuleType = mutationModule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.OnDemand
            });
        }
    }
}
