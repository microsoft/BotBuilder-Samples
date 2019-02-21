using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;
using System.Management.Automation;
using System.IO;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.Build.Evaluation;
using System.Linq;
using System.Reflection;

namespace DialogsTemplateVSIX
{
    public class WizardImplementation : IWizard
    {
        private string projectPath;
        private string folder;
        private string dialogsName;

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        // This method is called before opening any item that   
        // has the OpenInEditor attribute.  
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.  
        public void RunFinished()
        {
            string botClass = string.Empty;

            string[] fileNames = Directory.GetFiles(folder, "*.cs");

            string botFile = string.Empty;

            foreach (string file in fileNames)
            {
                using (StreamReader sReader = new StreamReader(file))
                {
                    string contents = sReader.ReadToEnd();
                    if (contents.Contains(": ComponentDialog"))
                    {
                        dialogsName = (Path.GetFileName(file)).Split('.')[0];
                    }
                    else
                    {
                        if (contents.Contains(": IBot"))
                        {
                            botFile = (Path.GetFileName(file)).Split('.')[0];
                        }
                    }
                }
            }

            if (botFile.Length >= 1)
            {
                botClass = botFile;
            }
            else
            {
                botClass = "BotClass";
            }

            RunScript(botClass, dialogsName);
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
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

                EnvDTE.Project selectedProject = selectedObject as EnvDTE.Project;

                this.projectPath = selectedProject.FullName;
                folder = Path.GetDirectoryName(projectPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        public void RunScript(string botClass, string dialogsName)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(File.ReadAllText(AssemblyDirectory + ".\\scripts\\UpdateBotClass.ps1"));
                PowerShellInstance.AddArgument(botClass);
                PowerShellInstance.AddArgument(dialogsName);
                PowerShellInstance.Runspace.SessionStateProxy.Path.SetLocation(folder);

                // begin invoke execution on the pipeline
                IAsyncResult result = PowerShellInstance.BeginInvoke();

                // do something else until execution has completed.
                // this could be sleep/wait, or perhaps some other work
                while (result.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");

                    // might want to place a timeout here...
                }

                Console.WriteLine("Finished!");
            }

            using (PowerShell PowerShellInstance1 = PowerShell.Create())
            {
                PowerShellInstance1.AddScript(File.ReadAllText(AssemblyDirectory + ".\\scripts\\UpdateStartUpClass.ps1"));
                PowerShellInstance1.AddArgument(botClass);
                PowerShellInstance1.Runspace.SessionStateProxy.Path.SetLocation(folder);

                // begin invoke execution on the pipeline
                IAsyncResult result = PowerShellInstance1.BeginInvoke();

                // do something else until execution has completed.
                // this could be sleep/wait, or perhaps some other work
                while (result.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");

                    // might want to place a timeout here...
                }

                Console.WriteLine("Finished!");
            }
        }
    }
}

