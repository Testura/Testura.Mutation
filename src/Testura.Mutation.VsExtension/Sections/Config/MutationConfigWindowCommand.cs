﻿using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace Testura.Mutation.VsExtension.Sections.Config
{
    internal sealed class MutationConfigWindowCommand
    {
        public const int CommandId = 4130;
        public static readonly Guid CommandSet = new Guid("eb065996-2187-4c2c-8ecf-947ac6264c49");

        private readonly AsyncPackage _package;

        private MutationConfigWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static MutationConfigWindowCommand Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MutationConfigWindowCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _package.JoinableTaskFactory.RunAsync(async () =>
            {
                try
                {
                    var window =
                        await _package.FindToolWindowAsync(typeof(MutationConfigWindow), 0, true, _package.DisposalToken)
                            as MutationConfigWindow;

                    if (window?.Frame == null)
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }

                    await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var windowFrame = (IVsWindowFrame)window.Frame;
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
                }
                catch (Exception ex)
                {
                    var o = ex.Message;
                }
            });
        }
    }
}