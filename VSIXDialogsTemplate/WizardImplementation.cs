using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;
using System.Windows.Forms;
using EnvDTE;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Linq;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;
using System.IO.Packaging;

namespace VSIXDialogsTemplate
{
    public class WizardImplementation : IWizard
    {
        private UserInputForm inputForm;
        private string customMessage;

        // This method is called before opening any item that   
        // has the OpenInEditor attribute.  
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        public void ProjectFinishedGenerating(Project project)
        {
            //var script =
            //project.ProjectItems.FindProjectItem(
            //    item => item.Name.Equals("C:\\Repositories\\BotBuilder-Samples\\samples\\csharp_dotnetcore\\02.a.echo-bot\\script.ps1"));

            //if (script == null)
            //{
            //    return;
            //}

            //var process =
            //    System.Diagnostics.Process.Start(
            //        "powershell",
            //        string.Concat(
            //            "-NoProfile -ExecutionPolicy Unrestricted -File \"",
            //            script.FileNames[0],
            //            "\""));
        }

        // This method is only called for item templates,  
        // not for project templates.  
        public void ProjectItemFinishedGenerating(ProjectItem
            projectItem)
        {
        }

        // This method is called after the project is created.  
        public void RunFinished()
        {
        }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            try
            {
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
            RunScript();

            this.Close();
        }

        private void RunScript()
        {

            //PowerShell ps = PowerShell.Create(); //.AddCommand("Install-Package").AddParameter("Id", "BotBuilder.Dialogs").Invoke();
            //ps.AddCommand("Install-Package").AddParameter("Id", "BotBuilder.Dialogs");
            //ps.Runspace("C:\\Repositories\\BotBuilder-Samples\\samples\\csharp_dotnetcore\\01.console-echo");

            string script = "" +
                "$FileName = \"*Bot.cs\"" + "\n" +
                "$Patern = \"turnContext.Activity.Type == ActivityTypes.Message\"" + "\n" +
                "$FileOriginal = Get-Content $FileName" + "\n" +
                "[String[]] $FileModified = @()" + "\n" +
                "Foreach ($Line in $FileOriginal)" + "\n" +
                "{" + "\n" +
                "   $FileModified += $Line "+ "\n" +
                "   if ($Line -match $patern)" + "\n" +
                "   {" + "\n" +
                "       $foreach.movenext()" + "\n" +
                "       $FileModified += \"			{ \"" + "\n" +
                "       $FileModified += \"             var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);\"" + "\n" +
                "       $FileModified += \"\"" + "\n" +
                "       $FileModified += \"             var results = await dialogContext.ContinueDialogAsync(cancellationToken);\"" + "\n" +
                "       $FileModified += \"\"" + "\n" +
                "       $FileModified += \"             if (results.Status == DialogTurnStatus.Empty)\"" + "\n" +
                "       $FileModified += \"             {\"" + "\n" +
                "       $FileModified += \"                 await dialogContext.BeginDialogAsync(null, null, cancellationToken);\"" + "\n" +
                "       $FileModified += \"             }\"" + "\n" +
                "   }" + "\n" +
               "}" + "\n" +
               "Set-Content $fileName $FileModified";

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(script);
                PowerShellInstance.Runspace.SessionStateProxy.Path.SetLocation("C:\\Repositories\\BotBuilder-Samples\\samples\\csharp_dotnetcore\\01.console-echo");

                //PowerShell.exe - NoProfile - ExecutionPolicy Unrestricted - Command "& {Start-Process PowerShell -windowstyle hidden -ArgumentList '-NoProfile -ExecutionPolicy Unrestricted -noexit -File "$ScriptPath"' -Verb RunAs}"

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





            //// create Powershell runspace
            //Runspace runspace = RunspaceFactory.CreateRunspace();

            //// open it
            //runspace.Open();

            //// create a pipeline and feed it the script text
            //Pipeline pipeline = runspace.CreatePipeline();
            //pipeline.Commands.AddScript(script);

            //// add an extra command to transform the script
            //// output objects into nicely formatted strings

            //// remove this line to get the actual objects
            //// that the script returns. For example, the script

            //// "Get-Process" returns a collection
            //// of System.Diagnostics.Process instances.
            //pipeline.Commands.Add("Out-String");

            //// execute the script
            //Collection <PSObject> results = pipeline.Invoke();

            //// close the runspace
            //runspace.Close();

            //// convert the script result into a single string
            //StringBuilder stringBuilder = new StringBuilder();
            //foreach (PSObject obj in results)
            //{
            //    stringBuilder.AppendLine(obj.ToString());
            //}

            ////return stringBuilder.ToString();
        }

        //private Project project()
        //{
        //    ThreadHelper.ThrowIfNotOnUIThread();

        //    IntPtr hierarchyPointer, selectionContainerPointer;
        //    Object selectedObject = null;
        //    IVsMultiItemSelect multiItemSelect;
        //    uint projectItemId;

        //    IVsMonitorSelection monitorSelection =
        //            (IVsMonitorSelection)Package.GetGlobalService(
        //            typeof(SVsShellMonitorSelection));

        //    monitorSelection.GetCurrentSelection(out hierarchyPointer,
        //                                         out projectItemId,
        //                                         out multiItemSelect,
        //                                         out selectionContainerPointer);

        //    IVsHierarchy selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
        //                                         hierarchyPointer,
        //                                         typeof(IVsHierarchy)) as IVsHierarchy;

        //    if (selectedHierarchy != null)
        //    {
        //        ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
        //                                          projectItemId,
        //                                          (int)__VSHPROPID.VSHPROPID_ExtObject,
        //                                          out selectedObject));
        //    }

        //    return selectedObject as Project;
        //}

        //private string projectPath()
        //{
        //    ThreadHelper.ThrowIfNotOnUIThread();

        //    string projectName = project().FullName;
        //    return projectName.Remove(projectName.LastIndexOf('\\'));
        //}
    }
}
