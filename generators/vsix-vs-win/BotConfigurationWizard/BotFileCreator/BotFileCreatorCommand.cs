// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator
{
    using System;
    using System.ComponentModel.Design;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class BotFileCreatorCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("d217be97-bc71-44be-ae8e-9a297f2af054");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotFileCreatorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private BotFileCreatorCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static BotFileCreatorCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in BotFileCreatorCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new BotFileCreatorCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            /*
             * This code gets the Project's name from where the Wizard/Visual Studio Extension is being called
             * Is necessary to call these methods from the main thread
             */
            IntPtr hierarchyPointer, selectionContainerPointer;
            object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelect, out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
            }

            Project selectedProject = selectedObject as Project;

            string projectPath = selectedProject.FullName;

            // Adds the project name to the Settings File to use it later.
            GeneralSettings.Default.ProjectName = projectPath;

            // Starts the wizard
            var botFileCreatorWizard = new BotFileCreationWizard();

            botFileCreatorWizard.ShowDialog();

            selectedProject.DTE.ExecuteCommand("Project.UnloadProject");
            selectedProject.DTE.ExecuteCommand("Project.ReloadProject");
        }
    }
}
