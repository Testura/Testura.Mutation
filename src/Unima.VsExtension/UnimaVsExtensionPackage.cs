using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using log4net.Config;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Shell;
using Unima.Application.Logs;
using Unima.VsExtension.Sections.ToolsWindow;
using Unima.VsExtension.Services;
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
    [Guid(UnimaVsExtensionPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MutationToolWindow))]
    public sealed class UnimaVsExtensionPackage : AsyncPackage
    {
        private OutputLoggerService _outputLoggerService;
        private Bootstrapper _bootstrapper;

        public const string PackageGuidString = "eb1b49be-0389-4dee-995a-cf1854262fa9";

        public UnimaVsExtensionPackage()
        {
            _bootstrapper = new Bootstrapper();
            _bootstrapper.Run();

            XmlConfigurator.Configure(new FileInfo(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + @"\Log4Net.Config"));
            _outputLoggerService = new OutputLoggerService(new LogWatcher());
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _outputLoggerService.StartLogger();

            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await MutationToolWindowCommand.InitializeAsync(this);
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
