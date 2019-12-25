using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Testura.Mutation.Application.Logs;
using Testura.Mutation.Core.Solution;
using Testura.Mutation.VsExtension.Sections.MutationExplorer;
using Testura.Mutation.VsExtension.Sections.Selects;
using Testura.Mutation.VsExtension.Services;
using Testura.Mutation.VsExtension.Solution;
using Testura.Mutation.VsExtension.Wrappers;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MutationExplorerWindow), MultiInstances = false, DocumentLikeTool = true)]
    [ProvideToolWindow(typeof(Testura.Mutation.VsExtension.Sections.Config.MutationConfigWindow), MultiInstances = false, DocumentLikeTool = true)]
    public sealed class TesturaMutationVsExtensionPackage : AsyncPackage
    {
        public const string PackageGuidString = "eb1b49be-0389-4dee-995a-cf1854262fa9";
        public const string BaseConfigName = "TesturaMutationBaseConfig.json";

        private OutputLoggerService _outputLoggerService;
        private Bootstrapper _bootstrapper;

        public TesturaMutationVsExtensionPackage()
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ConfigureWithEnvironmentServicesAsync(cancellationToken);

            XmlConfigurator.Configure(new FileInfo(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + @"\Log4Net.Config"));
            _outputLoggerService = new OutputLoggerService(JoinableTaskFactory, new LogWatcher());
            _outputLoggerService.StartLogger();

            await MutationExplorerWindowCommand.InitializeAsync(this);
            await SelectProjectFileCommand.InitializeAsync(this, _bootstrapper.Container.Resolve<MutationFilterItemCreatorService>());
            await Sections.Config.MutationConfigWindowCommand.InitializeAsync(this);
            await SelectLineCommand.InitializeAsync(this, _bootstrapper.Container.Resolve<MutationFilterItemCreatorService>());
        }

        protected override WindowPane InstantiateToolWindow(Type toolWindowType) => (WindowPane)GetService(toolWindowType);

        protected override object GetService(Type serviceType)
        {
            if (_bootstrapper.Container.IsRegistered(serviceType))
            {
                return _bootstrapper.Container.Resolve(serviceType);
            }

            return base.GetService(serviceType);
        }

        private async Task ConfigureWithEnvironmentServicesAsync(CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));

            if (componentModel == null)
            {
                throw new Exception("WAAA");
            }

            var workspace = componentModel.GetService<VisualStudioWorkspace>();
            var dte = (DTE)await GetServiceAsync(typeof(DTE));
            var asyncPackage = (AsyncPackage)await GetServiceAsync(typeof(AsyncPackage));

            _bootstrapper.Container.RegisterInstance(new EnvironmentWrapper(dte, JoinableTaskFactory, asyncPackage, new UserNotificationService(asyncPackage, JoinableTaskFactory)));
            _bootstrapper.Container.RegisterInstance(typeof(ISolutionOpener), new VisualStudioSolutionOpener(workspace));
            _bootstrapper.Container.RegisterInstance(typeof(ISolutionBuilder), new VisualStudioSolutionBuilder(dte, JoinableTaskFactory));
        }
    }
}
