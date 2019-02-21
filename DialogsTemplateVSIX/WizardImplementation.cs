using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using Project = EnvDTE.Project;

namespace DialogsTemplateVSIX
{
    public class WizardImplementation : IWizard
    {
        private const string lineToFindForPackageReference = "<PackageReference";

        private string projectPath;
        private string folder;
        private string dialogsName;

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private string packages = AssemblyDirectory + "\\scripts\\packages.txt";
               
        private string scriptUpdateBotClass = AssemblyDirectory + "\\scripts\\UpdateBotClass.ps1";

        private string scriptUpdateStartUpClass = AssemblyDirectory + "\\scripts\\UpdateStartUpClass.ps1";

        private Project project()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IntPtr hierarchyPointer, selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            IVsMonitorSelection monitorSelection =
                    (IVsMonitorSelection)Package.GetGlobalService(
                    typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer,
                                                 out projectItemId,
                                                 out multiItemSelect,
                                                 out selectionContainerPointer);

            IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                                                 hierarchyPointer,
                                                 typeof(IVsHierarchy)) as IVsHierarchy;

            if (selectedHierarchy != null)
            {
                ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                                                  projectItemId,
                                                  (int)__VSHPROPID.VSHPROPID_ExtObject,
                                                  out selectedObject));
            }

            return selectedObject as Project;
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                folder = Path.GetDirectoryName(project().FullName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // This method is called before opening any item that   
        // has the OpenInEditor attribute.  
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
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
            string[] fileNames = Directory.GetFiles(folder, "*.cs");

            string botFile = "BotClass";

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

            RunPowerShellInstance(scriptUpdateBotClass, botFile, dialogsName);

            RunPowerShellInstance(scriptUpdateStartUpClass, botFile);

            string[] pathEditFile = Directory.GetFiles(folder, "*.csproj");

            if (!(File.ReadAllLines(pathEditFile[0])
                .Any(line => line.Contains("Microsoft.Bot.Builder.Dialogs"))))
            {
                AddNuget(folder, pathEditFile[0]);
            }
        }

        public void RunPowerShellInstance(string script, params string[] arguments)
        {
            PowerShell PowerShellInstance = PowerShell.Create();

            PowerShellInstance.AddScript(File.ReadAllText(script));

            foreach (string argument in arguments)
            {
                PowerShellInstance.AddArgument(argument);
            }

            PowerShellInstance.Runspace.SessionStateProxy.Path.SetLocation(folder);

            PowerShellInstance.BeginInvoke();
        }

        private void AddNuget(string path, string pathEditFile)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            addLines(".csproj", packages, lineToFindForPackageReference, pathEditFile);

            var command = "/C cd" + path + "&& nuget restore" + path + "packages.config";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo = startInfo;
            process.Start();
        }

        private void addLines(string fileType, string fileToRead, string linesToFind, string pathEditFile)
        {
            string[] linesToAdd = File.ReadAllLines(fileToRead);
            string[] fileToEdit = File.ReadAllLines(pathEditFile);
            int indexOfBotfile = Array.FindIndex(fileToEdit, line => line.Contains(linesToFind));

            var firstPart = fileToEdit.Take(indexOfBotfile + 2).ToArray();
            var secondPart = fileToEdit.Skip(indexOfBotfile + 2).ToArray();

            var myNewFile = ConcatArrays(firstPart, linesToAdd, secondPart);

            File.WriteAllLines(pathEditFile, myNewFile);
        }

        private static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }       
    }
}

