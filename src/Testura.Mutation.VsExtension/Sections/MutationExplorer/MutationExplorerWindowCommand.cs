using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Testura.Mutation.VsExtension.Services;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension.Sections.MutationExplorer
{
    internal sealed class MutationExplorerWindowCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("eb065996-2187-4c2c-8ecf-947ac6264c49");

        private readonly AsyncPackage _package;
        private readonly UserNotificationService _userNotificationService;

        private MutationExplorerWindowCommand(AsyncPackage package, OleMenuCommandService commandService, UserNotificationService userNotificationService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _userNotificationService = userNotificationService;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static MutationExplorerWindowCommand Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package, UserNotificationService userNotificationService)
        {
            // Switch to the main thread - the call to AddCommand in MutationToolWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MutationExplorerWindowCommand(package, commandService, userNotificationService);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    var window = await _package.FindToolWindowAsync(typeof(MutationExplorerWindow), 0, true, _package.DisposalToken) as MutationExplorerWindow;

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
