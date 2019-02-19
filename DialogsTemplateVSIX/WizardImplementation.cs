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

namespace DialogsTemplateVSIX
{
    public class WizardImplementation : IWizard
    {
        private UserInputForm inputForm;
        private string customMessage;
        private string projectPath;
        private string folder;
        private string dialogsName;

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

                // Display a form to the user. The form collects   
                // input for the custom message.  
                inputForm = new UserInputForm();
                inputForm.ShowDialog();

                customMessage = UserInputForm.CustomMessage;

                // Add custom parameters.  
                replacementsDictionary.Add("$custommessage$",
                    customMessage);
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
                PowerShellInstance.AddScript(File.ReadAllText(".\\scripts\\DialogsContext.ps1"));
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
                    //Thread.Sleep(1000);

                    // might want to place a timeout here...
                }

                Console.WriteLine("Finished!");
            }

            using (PowerShell PowerShellInstance1 = PowerShell.Create())
            {
                PowerShellInstance1.AddScript(File.ReadAllText(".\\scripts\\DialogsRegisterAccesors.ps1"));
                PowerShellInstance1.AddArgument(botClass);
                PowerShellInstance1.Runspace.SessionStateProxy.Path.SetLocation(folder);

                // begin invoke execution on the pipeline
                IAsyncResult result = PowerShellInstance1.BeginInvoke();

                // do something else until execution has completed.
                // this could be sleep/wait, or perhaps some other work
                while (result.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");
                    //Thread.Sleep(1000);

                    // might want to place a timeout here...
                }

                Console.WriteLine("Finished!");
            }
        }
    }

    public partial class UserInputForm : Form
    {
        private static string customMessage;
        private TextBox textBox1;
        private Button button1;
        private Label label1;

        public UserInputForm()
        {
            this.Size = new System.Drawing.Size(1000, 700);
            this.Text = "Add New Dialog Wizard";

            label1 = new Label();
            label1.Text = "Enter bot file name";
            label1.Location = new System.Drawing.Point(20, 45);
            label1.Visible = true;
            this.Controls.Add(label1);

            textBox1 = new TextBox();
            textBox1.Location = new System.Drawing.Point(20, 85);
            textBox1.Size = new System.Drawing.Size(500, 50);
            this.Controls.Add(textBox1);

            button1 = new Button();
            button1.Location = new System.Drawing.Point(80, 85);
            button1.Size = new System.Drawing.Size(300, 50);
            button1.Text = "Add bot file";
            button1.Click += Button1_Click;
            this.Controls.Add(button1);
        }
        public static string CustomMessage
        {
            get
            {
                return customMessage;
            }
            set
            {
                customMessage = value;
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            customMessage = textBox1.Text;

            this.Close();
        }
    }
}

