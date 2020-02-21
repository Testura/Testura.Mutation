using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Testura.Mutation.VsExtension.Services;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    internal sealed class MutationConfigWindowCommand
    {
        public const int CommandId = 4130;
        public static readonly Guid CommandSet = new Guid("eb065996-2187-4c2c-8ecf-947ac6264c49");

        private readonly AsyncPackage _package;
        private readonly UserNotificationService _userNotificationService;

        private MutationConfigWindowCommand(AsyncPackage package, OleMenuCommandService commandService, UserNotificationService userNotificationService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _userNotificationService = userNotificationService;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static MutationConfigWindowCommand Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package, UserNotificationService userNotificationService)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MutationConfigWindowCommand(package, commandService, userNotificationService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    var window = await _package.FindToolWindowAsync(typeof(MutationConfigWindow), 0, true, _package.DisposalToken)
                            as MutationConfigWindow;

                    if (window?.Frame == null)
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }

                    await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var windowFrame = (IVsWindowFrame)window.Frame;

                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                    window.InitializeWindow();
                }
                catch (Exception)
                {
                    _userNotificationService.ShowError("Failed to open config window. Make sure that you have the latest visual studio update.");
                    throw;
                }
            });
        }
    }
}
