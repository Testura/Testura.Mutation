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
using Unima.Application.Logs;
using Unima.Core.Solution;
using Unima.VsExtension.Sections.MutationExplorer;
using Unima.VsExtension.Services;
using Unima.VsExtension.Solution;
using Task = System.Threading.Tasks.Task;

namespace Unima.VsExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MutationExplorerWindow))]
    [ProvideToolWindow(typeof(Unima.VsExtension.Sections.Config.UnimaConfigWindow))]
    public sealed class UnimaVsExtensionPackage : AsyncPackage
    {
        public const string PackageGuidString = "eb1b49be-0389-4dee-995a-cf1854262fa9";

        private OutputLoggerService _outputLoggerService;
        private Bootstrapper _bootstrapper;

        public UnimaVsExtensionPackage()
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));

            if (componentModel == null)
            {
                throw new Exception("WAAA");
            }

            var workspace = componentModel.GetService<VisualStudioWorkspace>();
            var dte = (DTE)await GetServiceAsync(typeof(DTE));

            XmlConfigurator.Configure(new FileInfo(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + @"\Log4Net.Config"));
            _outputLoggerService = new OutputLoggerService(JoinableTaskFactory, new LogWatcher());

            _bootstrapper.Container.RegisterInstance(typeof(ISolutionOpener), new VisualStudioSolutionOpener(workspace));
            _bootstrapper.Container.RegisterInstance(typeof(ISolutionBuilder), new VisualStudioSolutionBuilder(dte, JoinableTaskFactory));

            _outputLoggerService.StartLogger();

            await MutationExplorerWindowCommand.InitializeAsync(this);
            await Sections.SelectProjectFile.SelectProjectFileCommand.InitializeAsync(this);
            await Sections.Config.UnimaConfigWindowCommand.InitializeAsync(this);
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
    }
}
